import * as React from "react";
import { RiSearchLine, RiCloseLine, RiExternalLinkLine } from "@remixicon/react";

import { Input } from "@/components/ui/input";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { CONTRIBUTION_EMOJI, CONTRIBUTION_LABEL } from "@/lib/contributions";
import { cn } from "@/lib/utils";

export interface ContributorData {
  login: string;
  name: string;
  avatarUrl: string;
  profile: string;
  contributions: string[];
}

type SortKey =
  | "name-asc"
  | "name-desc"
  | "contributions-desc"
  | "contributions-asc";

const SORT_OPTIONS: { value: SortKey; label: string }[] = [
  { value: "name-asc", label: "Name (A-Z)" },
  { value: "name-desc", label: "Name (Z-A)" },
  { value: "contributions-desc", label: "Most contribution types" },
  { value: "contributions-asc", label: "Fewest contribution types" },
];

const SKELETON_COUNT = 10;

export interface ContributorsExplorerProps {
  contributors: ContributorData[];
}

/**
 * Client-side search, contribution-type filtering, and sorting over the
 * contributors roster. When no contributor matches the current filters, a
 * static (non-animated) skeleton grid is rendered in the exact shape of a
 * real results grid, rather than collapsing to an empty state.
 */
export function ContributorsExplorer({ contributors }: ContributorsExplorerProps) {
  const [query, setQuery] = React.useState("");
  const [activeTypes, setActiveTypes] = React.useState<Set<string>>(
    () => new Set(),
  );
  const [sortKey, setSortKey] = React.useState<SortKey>("name-asc");

  const availableTypes = React.useMemo(
    () =>
      Array.from(new Set(contributors.flatMap((c) => c.contributions))).sort(
        (a, b) => a.localeCompare(b),
      ),
    [contributors],
  );

  function toggleType(type: string) {
    setActiveTypes((prev) => {
      const next = new Set(prev);
      if (next.has(type)) {
        next.delete(type);
      } else {
        next.add(type);
      }
      return next;
    });
  }

  const filtered = React.useMemo(() => {
    const q = query.trim().toLowerCase();
    return contributors.filter((c) => {
      const matchesQuery =
        q.length === 0 ||
        c.name.toLowerCase().includes(q) ||
        c.login.toLowerCase().includes(q);
      const matchesTypes =
        activeTypes.size === 0 ||
        c.contributions.some((t) => activeTypes.has(t));
      return matchesQuery && matchesTypes;
    });
  }, [contributors, query, activeTypes]);

  const sorted = React.useMemo(() => {
    const copy = [...filtered];
    copy.sort((a, b) => {
      switch (sortKey) {
        case "name-asc":
          return a.name.localeCompare(b.name);
        case "name-desc":
          return b.name.localeCompare(a.name);
        case "contributions-desc":
          return (
            b.contributions.length - a.contributions.length ||
            a.name.localeCompare(b.name)
          );
        case "contributions-asc":
          return (
            a.contributions.length - b.contributions.length ||
            a.name.localeCompare(b.name)
          );
        default:
          return 0;
      }
    });
    return copy;
  }, [filtered, sortKey]);

  const hasResults = sorted.length > 0;

  return (
    <div>
      <div className="mb-8 flex flex-col gap-4">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
          <div className="relative flex-1">
            <RiSearchLine
              className="pointer-events-none absolute left-2.5 top-1/2 size-4 -translate-y-1/2 text-muted-foreground"
              aria-hidden="true"
            />
            <Input
              type="text"
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder="Search by name or GitHub handle"
              aria-label="Search contributors"
              className="pl-8 pr-8"
            />
            {query.length > 0 ? (
              <button
                type="button"
                onClick={() => setQuery("")}
                aria-label="Clear search"
                className="absolute right-1 top-1/2 inline-flex size-7 -translate-y-1/2 cursor-pointer items-center justify-center rounded-md text-muted-foreground transition hover:bg-accent hover:text-foreground"
              >
                <RiCloseLine className="size-4" aria-hidden="true" />
              </button>
            ) : null}
          </div>
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <span className="shrink-0">Sort by</span>
            <Select
              value={sortKey}
              onValueChange={(value) => setSortKey(value as SortKey)}
            >
              <SelectTrigger aria-label="Sort contributors">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {SORT_OPTIONS.map((opt) => (
                  <SelectItem key={opt.value} value={opt.value}>
                    {opt.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        <div
          className="flex flex-wrap items-center gap-2"
          role="group"
          aria-label="Filter by contribution type"
        >
          <span className="text-sm font-medium text-foreground">Key:</span>
          {availableTypes.map((type) => {
            const active = activeTypes.has(type);
            return (
              <button
                key={type}
                type="button"
                aria-pressed={active}
                onClick={() => toggleType(type)}
                className={cn(
                  "inline-flex h-9 cursor-pointer select-none items-center gap-1.5 rounded-full border px-3.5 text-xs font-medium transition active:scale-[0.98]",
                  active
                    ? "border-primary bg-primary/10 text-primary"
                    : "border-border/60 text-muted-foreground hover:border-primary/40 hover:bg-accent hover:text-foreground"
                )}
              >
                <span aria-hidden="true">{CONTRIBUTION_EMOJI[type] ?? "✨"}</span>
                {CONTRIBUTION_LABEL[type] ?? type}
              </button>
            );
          })}
          {activeTypes.size > 0 ? (
            <button
              type="button"
              onClick={() => setActiveTypes(new Set())}
              className="inline-flex h-9 cursor-pointer items-center px-2 text-xs font-medium text-muted-foreground underline-offset-4 hover:text-foreground hover:underline"
            >
              Clear filters
            </button>
          ) : null}
          <a
            href="https://allcontributors.org/en/reference/emoji-key/"
            target="_blank"
            rel="noreferrer"
            className="ml-auto inline-flex h-9 cursor-pointer items-center gap-1 px-2 text-xs font-medium text-foreground underline-offset-4 hover:underline"
          >
            Full emoji key
            <RiExternalLinkLine className="size-3.5" aria-hidden="true" />
          </a>
        </div>

        <p className="text-sm text-muted-foreground" aria-live="polite">
          {hasResults
            ? `Showing ${sorted.length} of ${contributors.length} contributors.`
            : "No contributors match your filters."}
        </p>
      </div>

      {hasResults ? (
        <ul
          className="grid list-none grid-cols-2 gap-4 p-0 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5"
          aria-label="Project contributors"
        >
          {sorted.map((contributor) => (
            <li key={contributor.login}>
              <a
                href={contributor.profile}
                target="_blank"
                rel="noreferrer"
                className="group flex h-full flex-col items-center gap-2 rounded-2xl border border-border/60 bg-background p-4 text-center shadow-sm transition hover:border-primary/40 hover:shadow-md"
              >
                <img
                  src={contributor.avatarUrl}
                  alt=""
                  aria-hidden="true"
                  width={72}
                  height={72}
                  loading="lazy"
                  decoding="async"
                  className="size-[72px] rounded-full"
                />
                <span className="text-sm font-semibold tracking-tight text-foreground group-hover:text-primary">
                  {contributor.name}
                </span>
                <span
                  className="text-base"
                  title={contributor.contributions.join(", ")}
                  aria-label={`Contributions: ${contributor.contributions.join(", ")}`}
                >
                  {contributor.contributions
                    .map((c) => CONTRIBUTION_EMOJI[c] ?? "✨")
                    .join(" ")}
                </span>
              </a>
            </li>
          ))}
        </ul>
      ) : (
        <ul
          className="grid list-none grid-cols-2 gap-4 p-0 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5"
          aria-hidden="true"
        >
          {Array.from({ length: SKELETON_COUNT }).map((_, index) => (
            <li key={index}>
              <div className="flex h-full flex-col items-center gap-2 rounded-2xl border border-border/60 bg-background p-4 text-center shadow-sm">
                <Skeleton className="size-[72px] animate-none rounded-full" />
                <Skeleton className="h-4 w-20 animate-none rounded" />
                <Skeleton className="h-4 w-14 animate-none rounded" />
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
