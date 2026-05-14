import { describe, expect, it, beforeEach } from "vitest";
import { applyTheme, readTheme } from "@/components/theme-toggle";

describe("theme persistence helpers", () => {
  beforeEach(() => {
    document.documentElement.className = "";
    document.documentElement.removeAttribute("style");
    localStorage.clear();
  });

  it("applies the dark class on the documentElement when set to dark", () => {
    applyTheme("dark");
    expect(document.documentElement.classList.contains("dark")).toBe(true);
    expect(document.documentElement.style.colorScheme).toBe("dark");
  });

  it("removes the dark class when set to light", () => {
    applyTheme("dark");
    applyTheme("light");
    expect(document.documentElement.classList.contains("dark")).toBe(false);
    expect(document.documentElement.style.colorScheme).toBe("light");
  });

  it("persists the choice in localStorage", () => {
    applyTheme("dark");
    expect(localStorage.getItem("cosmos-repo-theme")).toBe("dark");
    applyTheme("light");
    expect(localStorage.getItem("cosmos-repo-theme")).toBe("light");
  });

  it("readTheme reflects the documentElement class", () => {
    expect(readTheme()).toBe("light");
    document.documentElement.classList.add("dark");
    expect(readTheme()).toBe("dark");
  });
});
