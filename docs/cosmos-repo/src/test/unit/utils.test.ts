import { describe, expect, it } from "vitest";
import { withBase, BASE, cn } from "@/lib/utils";

describe("withBase", () => {
  it("prefixes the configured GH Pages base path", () => {
    expect(withBase("/docs/getting-started")).toBe(
      `${BASE}/docs/getting-started`,
    );
  });

  it("adds a leading slash when missing", () => {
    expect(withBase("docs/queries")).toBe(`${BASE}/docs/queries`);
  });

  it("returns absolute URLs unchanged", () => {
    const url = "https://www.nuget.org/packages/IEvangelist.Azure.CosmosRepository";
    expect(withBase(url)).toBe(url);
  });

  it("handles the root path", () => {
    expect(withBase("/")).toBe(`${BASE}/`);
  });
});

describe("cn", () => {
  it("merges duplicate Tailwind utilities, keeping the last", () => {
    expect(cn("px-2", "px-4")).toBe("px-4");
  });

  it("filters falsy values", () => {
    expect(cn("foo", false, undefined, null, "bar")).toBe("foo bar");
  });
});
