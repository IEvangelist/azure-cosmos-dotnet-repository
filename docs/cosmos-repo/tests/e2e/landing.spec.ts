import { test, expect } from "@playwright/test";
import AxeBuilder from "@axe-core/playwright";

const BASE_PATH = "/azure-cosmos-dotnet-repository";

test.describe("Landing page", () => {
  test("renders the hero and primary CTA", async ({ page }) => {
    await page.goto(`${BASE_PATH}/`);
    await expect(
      page.getByRole("heading", { name: /repository.*way/i, level: 1 }),
    ).toBeVisible();
    await expect(page.getByRole("link", { name: /get started/i })).toBeVisible();
  });

  test("CTA navigates to the docs", async ({ page }) => {
    await page.goto(`${BASE_PATH}/`);
    await page
      .getByRole("link", { name: /get started/i })
      .first()
      .click();
    await expect(page).toHaveURL(new RegExp(`${BASE_PATH}/docs/getting-started`));
    await expect(
      page.getByRole("heading", { name: /getting started/i, level: 1 }),
    ).toBeVisible();
  });

  test("has no critical accessibility violations", async ({ page }) => {
    await page.goto(`${BASE_PATH}/`);
    const results = await new AxeBuilder({ page })
      .withTags(["wcag2a", "wcag2aa", "wcag21a", "wcag21aa"])
      .analyze();
    const blocking = results.violations.filter(
      (v) => v.impact === "critical" || v.impact === "serious",
    );
    expect(blocking, JSON.stringify(blocking, null, 2)).toEqual([]);
  });
});
