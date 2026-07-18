import { describe, expect, it } from "vitest";
import { docsNav, flattenNav, findNeighbors } from "@/lib/nav";

describe("docsNav", () => {
  it("starts every URL with /docs/", () => {
    for (const section of docsNav) {
      expect(section.url.startsWith("/docs/")).toBe(true);
      for (const item of section.items ?? []) {
        expect(item.url.startsWith("/docs/")).toBe(true);
      }
    }
  });

  it("has a unique URL per leaf", () => {
    const urls = flattenNav().map((i) => i.url);
    expect(new Set(urls).size).toBe(urls.length);
  });
});

describe("flattenNav", () => {
  it("returns the leaves of every section", () => {
    const leaves = flattenNav();
    expect(leaves.length).toBeGreaterThan(0);
    expect(leaves.every((leaf) => !leaf.items?.length)).toBe(true);
  });
});

describe("findNeighbors", () => {
  it("returns undefined neighbors for an unknown path", () => {
    const { prev, next } = findNeighbors("/docs/unknown");
    expect(prev).toBeUndefined();
    expect(next).toBeUndefined();
  });

  it("returns the next leaf when on the first one", () => {
    const flat = flattenNav();
    const first = flat[0];
    const { prev, next } = findNeighbors(first.url);
    expect(prev).toBeUndefined();
    expect(next?.url).toBe(flat[1].url);
  });

  it("returns prev/next around an interior leaf", () => {
    const flat = flattenNav();
    const target = flat[2];
    const { prev, next } = findNeighbors(target.url);
    expect(prev?.url).toBe(flat[1].url);
    expect(next?.url).toBe(flat[3].url);
  });

  it("returns the previous leaf when on the last one", () => {
    const flat = flattenNav();
    const last = flat[flat.length - 1];
    const { prev, next } = findNeighbors(last.url);
    expect(next).toBeUndefined();
    expect(prev?.url).toBe(flat[flat.length - 2].url);
  });

  it("ignores trailing slashes", () => {
    const flat = flattenNav();
    const target = flat[1];
    const { prev: a } = findNeighbors(target.url);
    const { prev: b } = findNeighbors(`${target.url}/`);
    expect(a?.url).toBe(b?.url);
  });
});
