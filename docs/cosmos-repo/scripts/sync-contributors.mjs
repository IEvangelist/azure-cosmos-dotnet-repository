#!/usr/bin/env node
/**
 * Syncs contributor data from the repo-root `.all-contributorsrc` file into
 * `src/data/contributors.json` so the Astro site can render the dedicated
 * contributors page without reaching outside its Vite project root.
 *
 * Usage:
 *   node scripts/sync-contributors.mjs
 */

import { readFile, writeFile, mkdir } from "node:fs/promises";
import { fileURLToPath } from "node:url";
import { dirname, join, relative, resolve } from "node:path";

const __dirname = dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = resolve(__dirname, "..", "..", "..");
const SOURCE_PATH = join(REPO_ROOT, ".all-contributorsrc");
const OUT_DIR = resolve(__dirname, "..", "src", "data");
const OUT_PATH = join(OUT_DIR, "contributors.json");

const raw = await readFile(SOURCE_PATH, "utf8");
const { contributors } = JSON.parse(raw);

const data = contributors.map(
  ({ login, name, avatar_url: avatarUrl, profile, contributions }) => ({
    login,
    name,
    avatarUrl,
    profile,
    contributions,
  }),
);

await mkdir(OUT_DIR, { recursive: true });
await writeFile(OUT_PATH, `${JSON.stringify(data, null, 2)}\n`, "utf8");

console.log(
  `[sync-contributors] Wrote ${data.length} contributors to ${relative(REPO_ROOT, OUT_PATH)}`,
);
