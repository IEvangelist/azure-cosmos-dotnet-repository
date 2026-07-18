import { test, expect } from "@playwright/test";
import AxeBuilder from "@axe-core/playwright";

const BASE_PATH = "/azure-cosmos-dotnet-repository";

test.describe("Docs shell", () => {
  test("sidebar navigation jumps between pages", async ({ page }) => {
    await page.goto(`${BASE_PATH}/docs/getting-started`);
    await expect(
      page.getByRole("heading", { name: /getting started/i, level: 1 }),
    ).toBeVisible();
    await page.getByRole("link", { name: "Authentication" }).first().click();
    await expect(page).toHaveURL(
      new RegExp(`${BASE_PATH}/docs/getting-started/authentication`),
    );
    await expect(
      page.getByRole("heading", { name: /authentication/i, level: 1 }),
    ).toBeVisible();
  });

  test("theme toggle persists across reload", async ({ page }) => {
    await page.goto(`${BASE_PATH}/docs/getting-started`);
    const html = page.locator("html");
    await expect(html).not.toHaveClass(/dark/);
    await page.getByRole("button", { name: /switch to dark theme/i }).click();
    await expect(html).toHaveClass(/dark/);
    await page.reload();
    await expect(page.locator("html")).toHaveClass(/dark/);
  });

  test("skip link is reachable and focuses the main region", async ({ page }) => {
    await page.goto(`${BASE_PATH}/docs/getting-started`);
    await page.keyboard.press("Tab");
    const skip = page.getByRole("link", { name: /skip to main content/i });
    await expect(skip).toBeFocused();
    await skip.press("Enter");
    await expect(page.locator("main#main")).toBeVisible();
  });

  test("a docs page has no critical accessibility violations", async ({ page }) => {
    await page.goto(`${BASE_PATH}/docs/change-feed`);
    const results = await new AxeBuilder({ page })
      .withTags(["wcag2a", "wcag2aa", "wcag21a", "wcag21aa"])
      .analyze();
    const blocking = results.violations.filter(
      (v) => v.impact === "critical" || v.impact === "serious",
    );
    expect(blocking, JSON.stringify(blocking, null, 2)).toEqual([]);
  });
});
