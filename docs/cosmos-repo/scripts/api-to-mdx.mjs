#!/usr/bin/env node
/**
 * docfx YAML → MDX converter.
 *
 * Reads `*.yml` files emitted by `docfx metadata` into `api-yml/` and writes
 * Astro MDX pages into `src/content/docs/api/`. One page per top-level type
 * (Class / Interface / Struct / Enum / Delegate), with all members rendered
 * on the same page. Cross-refs are rendered as inline code (we don't try to
 * resolve external links to the BCL or third-party packages).
 *
 * Usage:
 *   node scripts/api-to-mdx.mjs            # default in/out
 *   node scripts/api-to-mdx.mjs --in=api-yml --out=src/content/docs/api
 */

import { readFileSync, readdirSync, writeFileSync, mkdirSync, rmSync, existsSync } from "node:fs";
import { join, dirname } from "node:path";
import { fileURLToPath } from "node:url";
import { parse } from "yaml";

const __dirname = dirname(fileURLToPath(import.meta.url));
const ROOT = join(__dirname, "..");

const args = Object.fromEntries(
  process.argv.slice(2).map((a) => {
    const [k, v] = a.replace(/^--/, "").split("=");
    return [k, v ?? true];
  }),
);
const IN_DIR = join(ROOT, args.in || "api-yml");
const OUT_DIR = join(ROOT, args.out || "src/content/docs/api");

if (!existsSync(IN_DIR)) {
  console.error(`[api-to-mdx] Input directory not found: ${IN_DIR}`);
  console.error("[api-to-mdx] Run `docfx metadata docfx.json` first.");
  process.exit(1);
}

/* ------------------------------------------------------------------ */
/* Helpers                                                            */
/* ------------------------------------------------------------------ */

function slugifyType(fullName) {
  return fullName
    .replace(/`\d+/g, "")
    .replace(/[<>]/g, "")
    .replace(/\s+/g, "")
    .replace(/\./g, "-")
    .toLowerCase();
}

function slugifyMember(uid) {
  return uid
    .replace(/`\d+/g, "")
    .replace(/[^a-zA-Z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase()
    .slice(0, 80);
}

function escapeMdx(text) {
  if (!text) return "";
  return text
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/\{/g, "\\{")
    .replace(/\}/g, "\\}");
}

function renderInline(text) {
  if (!text) return "";
  let out = String(text);

  out = out.replace(/<xref\s+href="([^"]+)"[^>]*>\s*<\/xref>/g, (_m, href) => {
    const decoded = decodeURIComponent(href).replace(/`\d+/g, "");
    const short = decoded.split(".").pop().replace(/%60\d+/gi, "");
    return "`" + short + "`";
  });

  out = out.replace(/<see\s+cref="([^"]+)"\s*\/?>(\s*<\/see>)?/g, (_m, cref) => {
    const cleaned = cref.replace(/^[A-Z]:/, "").replace(/`\d+/g, "");
    const short = cleaned.split(".").pop();
    return "`" + short + "`";
  });

  out = out.replace(/<c>([^<]+)<\/c>/g, (_m, c) => "`" + decodeEntities(c) + "`");
  out = out.replace(
    /<code(?:\s+class="[^"]*")?>([^<\n]+)<\/code>/g,
    (_m, c) => "`" + decodeEntities(c) + "`",
  );

  out = out.replace(/<typeparamref\s+name="([^"]+)"\s*\/?>(\s*<\/typeparamref>)?/g, "`$1`");
  out = out.replace(/<paramref\s+name="([^"]+)"\s*\/?>(\s*<\/paramref>)?/g, "`$1`");

  out = out.replace(/<\/?para>/g, "\n\n");
  out = out.replace(/<br\s*\/?>/g, "\n");

  out = out.replace(
    /<pre>\s*<code(?:\s+class="lang-([^"]+)")?>([\s\S]+?)<\/code>\s*<\/pre>/g,
    (_m, lang, code) => {
      const langTag = (lang || "csharp").replace("c#", "csharp");
      const cleaned = decodeEntities(code);
      return "\n\n```" + langTag + "\n" + cleaned.trim() + "\n```\n\n";
    },
  );

  out = out.replace(/<\/?[a-zA-Z][^>]*>/g, "");

  out = out.replace(/[ \t]+/g, " ").replace(/\n[ \t]+/g, "\n");

  out = escapeBracesOutsideCode(out);

  return out.trim();
}

function escapeBracesOutsideCode(text) {
  const parts = String(text).split(/(```[\s\S]*?```)/g);
  return parts
    .map((part, i) => {
      if (i % 2 === 1) return part;
      return part.replace(/\{/g, "\\{").replace(/\}/g, "\\}");
    })
    .join("");
}

function decodeEntities(text) {
  return String(text)
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&")
    .replace(/&quot;/g, '"')
    .replace(/&#39;/g, "'");
}

/* ------------------------------------------------------------------ */
/* Load all type entries                                              */
/* ------------------------------------------------------------------ */

const yamlFiles = readdirSync(IN_DIR).filter(
  (f) => f.endsWith(".yml") && f !== "toc.yml",
);

const TYPES = [];

for (const file of yamlFiles) {
  const text = readFileSync(join(IN_DIR, file), "utf8");
  const yaml = text.replace(/^### YamlMime:[^\n]*\n/, "");
  const doc = parse(yaml);
  if (!doc || !Array.isArray(doc.items)) continue;

  const top = doc.items[0];
  if (!top) continue;
  const kind = top.type;
  if (kind === "Namespace") continue;
  if (!["Class", "Interface", "Struct", "Enum", "Delegate"].includes(kind)) continue;

  const members = doc.items.slice(1);

  TYPES.push({
    uid: top.uid,
    name: top.name,
    nameWithType: top.nameWithType,
    fullName: top.fullName,
    namespace: top.namespace,
    kind,
    summary: top.summary,
    example: top.example,
    remarks: top.remarks,
    syntax: top.syntax,
    typeParameters: top.syntax?.typeParameters,
    members,
    sourceFile: file,
  });
}

console.log(`[api-to-mdx] Loaded ${TYPES.length} types from ${yamlFiles.length} files`);

/* ------------------------------------------------------------------ */
/* Render                                                              */
/* ------------------------------------------------------------------ */

const KIND_LABEL = {
  Class: "class",
  Interface: "interface",
  Struct: "struct",
  Enum: "enum",
  Delegate: "delegate",
};

function memberSection(label, items) {
  if (!items?.length) return "";
  const lines = [`### ${label}`, ""];
  for (const m of items) {
    const sig = m.syntax?.content || m.nameWithType || m.name;
    const anchor = slugifyMember(m.uid);
    lines.push(`<a id="${anchor}"></a>`);
    lines.push("");
    lines.push(`#### ${escapeMdx(m.nameWithType || m.name)}`);
    lines.push("");
    if (m.summary) {
      lines.push(renderInline(m.summary));
      lines.push("");
    }
    if (sig) {
      lines.push("```csharp");
      lines.push(sig);
      lines.push("```");
      lines.push("");
    }
    if (m.syntax?.parameters?.length) {
      lines.push("**Parameters**");
      lines.push("");
      lines.push("| Name | Type | Description |");
      lines.push("| --- | --- | --- |");
      for (const p of m.syntax.parameters) {
        const t = (p.type || "").split(".").pop().replace(/`\d+/g, "");
        const d = renderInline(p.description || "").replace(/\n/g, " ").replace(/\|/g, "\\|");
        lines.push(`| \`${p.id}\` | \`${t}\` | ${d || "—"} |`);
      }
      lines.push("");
    }
    if (m.syntax?.return && m.syntax.return.type && m.syntax.return.type !== "System.Void") {
      const rt = m.syntax.return.type.split(".").pop().replace(/`\d+/g, "");
      const rd = renderInline(m.syntax.return.description || "");
      lines.push("**Returns** — `" + rt + "`" + (rd ? `: ${rd}` : ""));
      lines.push("");
    }
    if (m.remarks) {
      lines.push(renderInline(m.remarks));
      lines.push("");
    }
    if (m.example) {
      const examples = Array.isArray(m.example) ? m.example : [m.example];
      for (const ex of examples) {
        lines.push(renderInline(ex));
        lines.push("");
      }
    }
  }
  return lines.join("\n");
}

function renderTypePage(t) {
  const lines = [];
  const desc =
    renderInline(t.summary || "")
      .split("\n")[0]
      .replace(/"/g, "'")
      .slice(0, 180) || `${t.kind} ${t.name} in ${t.namespace}.`;
  const title = t.nameWithType.replace(/"/g, "'");
  lines.push("---");
  lines.push(`title: "${title}"`);
  lines.push(`description: "${desc}"`);
  lines.push("---");
  lines.push("");
  lines.push(`**Namespace:** \`${t.namespace}\`  `);
  lines.push(`**Kind:** ${KIND_LABEL[t.kind] || t.kind}`);
  lines.push("");
  if (t.summary) {
    lines.push(renderInline(t.summary));
    lines.push("");
  }
  if (t.syntax?.content) {
    lines.push("```csharp");
    lines.push(t.syntax.content);
    lines.push("```");
    lines.push("");
  }
  if (t.typeParameters?.length) {
    lines.push("**Type parameters**");
    lines.push("");
    lines.push("| Name | Description |");
    lines.push("| --- | --- |");
    for (const tp of t.typeParameters) {
      const d = renderInline(tp.description || "").replace(/\n/g, " ").replace(/\|/g, "\\|");
      lines.push(`| \`${tp.id}\` | ${d || "—"} |`);
    }
    lines.push("");
  }
  if (t.remarks) {
    lines.push("## Remarks");
    lines.push("");
    lines.push(renderInline(t.remarks));
    lines.push("");
  }
  if (t.example) {
    const examples = Array.isArray(t.example) ? t.example : [t.example];
    lines.push("## Example");
    lines.push("");
    for (const ex of examples) {
      lines.push(renderInline(ex));
      lines.push("");
    }
  }

  const byKind = {};
  for (const m of t.members) {
    (byKind[m.type] ??= []).push(m);
  }
  if (Object.keys(byKind).length) {
    lines.push("## Members");
    lines.push("");
  }
  const ORDER = ["Constructor", "Property", "Field", "Method", "Operator", "Event"];
  for (const k of ORDER) {
    if (byKind[k]) {
      lines.push(memberSection(`${k}${byKind[k].length > 1 ? "s" : ""}`, byKind[k]));
      lines.push("");
    }
  }

  return lines.join("\n");
}

/* ------------------------------------------------------------------ */
/* Write                                                              */
/* ------------------------------------------------------------------ */

if (existsSync(OUT_DIR)) {
  rmSync(OUT_DIR, { recursive: true, force: true });
}
mkdirSync(OUT_DIR, { recursive: true });

const byNs = new Map();
for (const t of TYPES) {
  if (!byNs.has(t.namespace)) byNs.set(t.namespace, []);
  byNs.get(t.namespace).push(t);
}
for (const arr of byNs.values()) {
  arr.sort((a, b) => a.name.localeCompare(b.name));
}

const indexLines = [];
indexLines.push("---");
indexLines.push(`title: "API reference"`);
indexLines.push(
  `description: "Generated reference for every public type in Microsoft.Azure.CosmosRepository and Microsoft.Azure.CosmosRepository.AspNetCore."`,
);
indexLines.push("---");
indexLines.push("");
  indexLines.push(
    "These pages are generated from the project's XML documentation comments via `docfx metadata`, then converted to MDX so they render with the same theme, search, and navigation as the rest of the docs. Re-run `pnpm api` to refresh.",
  );
  indexLines.push("");
const namespaces = [...byNs.keys()].sort();
for (const ns of namespaces) {
  indexLines.push(`## ${ns}`);
  indexLines.push("");
  indexLines.push(`<ul class="docs-card-grid">`);
  for (const t of byNs.get(ns)) {
    const slug = slugifyType(t.fullName);
    const desc = renderInline(t.summary || "")
      .split("\n")[0]
      .slice(0, 140)
      .replace(/\\\{/g, "{")
      .replace(/\\\}/g, "}")
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/\{/g, "&#123;")
      .replace(/\}/g, "&#125;");
    const name = t.nameWithType
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
    indexLines.push(
      `  <li><a href="./api/${slug}/"><strong>${name}</strong><br /><span class="text-sm text-muted-foreground">${desc}</span></a></li>`,
    );
  }
  indexLines.push("</ul>");
  indexLines.push("");
}

writeFileSync(join(OUT_DIR, "..", "api.mdx"), indexLines.join("\n"), "utf8");

let written = 0;
for (const t of TYPES) {
  const slug = slugifyType(t.fullName);
  writeFileSync(join(OUT_DIR, `${slug}.mdx`), renderTypePage(t), "utf8");
  written++;
}

console.log(`[api-to-mdx] Wrote 1 index + ${written} type pages to ${OUT_DIR}`);
