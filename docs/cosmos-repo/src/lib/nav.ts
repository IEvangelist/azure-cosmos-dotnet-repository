/**
 * Single source of truth for navigation. Used by both the Astro pages
 * (build-time link generation, breadcrumbs) and the React sidebar island.
 *
 * Order in the array IS the order shown in the sidebar.
 */
export type NavItem = {
  title: string;
  /** Application path with leading slash; we resolve the GH Pages base prefix at the component layer. */
  url: string;
  /** Lucide icon name (rendered by the React island; ignored on the server). */
  icon?: string;
  description?: string;
  items?: NavItem[];
};

export const docsNav: NavItem[] = [
  {
    title: "Getting started",
    url: "/docs/getting-started",
    icon: "Rocket",
    description:
      "Install the package, register the repository, and create your first item.",
    items: [
      {
        title: "Overview",
        url: "/docs/getting-started",
      },
      {
        title: "Authentication",
        url: "/docs/getting-started/authentication",
      },
      {
        title: "Partitioning",
        url: "/docs/getting-started/partitioning",
      },
    ],
  },
  {
    title: "Container configuration",
    url: "/docs/container-config",
    icon: "Boxes",
    description:
      "Configure container properties, throughput, and serialization.",
    items: [{ title: "Overview", url: "/docs/container-config" }],
  },
  {
    title: "Item types",
    url: "/docs/item-types",
    icon: "Layers",
    description:
      "Item, EtagItem, TimeStampedItem, TimeToLiveItem, and FullItem.",
    items: [
      { title: "Overview", url: "/docs/item-types" },
      { title: "Etags & OCC", url: "/docs/item-types/etags" },
      { title: "FullItem", url: "/docs/item-types/full-item" },
      { title: "Timestamps", url: "/docs/item-types/time-stamps" },
      { title: "Time to live", url: "/docs/item-types/time-to-live" },
    ],
  },
  {
    title: "Queries",
    url: "/docs/queries",
    icon: "Search",
    description: "Query items with predicates, paging, and the Specification pattern.",
    items: [
      { title: "Overview", url: "/docs/queries" },
      {
        title: "Specification pattern",
        url: "/docs/queries/specification-pattern",
      },
    ],
  },
  {
    title: "Change feed",
    url: "/docs/change-feed",
    icon: "Activity",
    description:
      "Process Cosmos DB change feed events with hosted background services.",
    items: [{ title: "Overview", url: "/docs/change-feed" }],
  },
  {
    title: "Miscellaneous",
    url: "/docs/misc",
    icon: "SlidersHorizontal",
    description:
      "Health checks, logging, unique keys, and container initialization.",
    items: [
      {
        title: "Container initialization",
        url: "/docs/misc/container-initialization",
      },
      { title: "Health checks", url: "/docs/misc/health-checks" },
      { title: "Logging", url: "/docs/misc/logging" },
      { title: "Unique keys", url: "/docs/misc/unique-keys" },
    ],
  },
  {
    title: "Release notes",
    url: "/docs/release-notes",
    icon: "BookOpen",
    description: "Major release notes and migration guidance.",
    items: [{ title: "Overview", url: "/docs/release-notes" }],
  },
];

/**
 * Flatten navigation for search indexing and prev/next link generation.
 * Each leaf is yielded once.
 */
export function flattenNav(items: NavItem[] = docsNav): NavItem[] {
  const out: NavItem[] = [];
  for (const item of items) {
    if (item.items?.length) {
      for (const child of item.items) {
        out.push(child);
      }
    } else {
      out.push(item);
    }
  }
  return out;
}

/**
 * Find prev/next neighbors for a given path (used in the doc page footer).
 * Returns undefined for either end of the list.
 */
export function findNeighbors(currentPath: string) {
  const flat = flattenNav();
  const normalized = currentPath.replace(/\/$/, "");
  const index = flat.findIndex(
    (i) => i.url.replace(/\/$/, "") === normalized,
  );
  if (index === -1) {
    return { prev: undefined, next: undefined };
  }
  return {
    prev: index > 0 ? flat[index - 1] : undefined,
    next: index < flat.length - 1 ? flat[index + 1] : undefined,
  };
}
