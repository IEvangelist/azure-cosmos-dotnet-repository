// @ts-check
import { defineConfig } from "astro/config";
import react from "@astrojs/react";
import mdx from "@astrojs/mdx";
import sitemap from "@astrojs/sitemap";
import expressiveCode from "astro-expressive-code";
import tailwindcss from "@tailwindcss/vite";

const SITE = "https://ievangelist.github.io";
const BASE = "/azure-cosmos-dotnet-repository";

// https://astro.build/config
export default defineConfig({
  site: SITE,
  base: BASE,
  trailingSlash: "ignore",
  output: "static",
  build: {
    format: "directory",
  },
  integrations: [
    react(),
    // Expressive Code must come before MDX so it can register its remark/rehype hooks.
    // Config lives in `ec.config.mjs` (required because of the function-valued `themeCssSelector`).
    expressiveCode(),
    mdx({
      gfm: true,
    }),
    sitemap(),
  ],
  vite: {
    plugins: [tailwindcss()],
    resolve: {
      alias: {
        "@": new URL("./src", import.meta.url).pathname,
      },
    },
  },
});
