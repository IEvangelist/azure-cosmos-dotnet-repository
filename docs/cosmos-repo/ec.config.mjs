// @ts-check
import { defineEcConfig } from "astro-expressive-code";

export default defineEcConfig({
  themes: ["github-dark", "github-light"],
  themeCssSelector: (theme) =>
    theme.name === "github-dark" ? "html.dark" : "html:not(.dark)",
  useDarkModeMediaQuery: false,
  // Inline the EC base + theme styles directly into each page instead of
  // emitting an external stylesheet. This avoids a known issue where the
  // generated <link> tag isn't injected into the rendered HTML.
  emitExternalStylesheet: false,
  styleOverrides: {
    borderRadius: "0.75rem",
    codeFontSize: "0.875rem",
    codeLineHeight: "1.6",
    frames: {
      shadowColor: "transparent",
      editorActiveTabBorderColor: "transparent",
      // Keep the copy button visible at rest so users see they can copy.
      inlineButtonForeground: "#ffffff",
      inlineButtonBackground: "#ffffff",
      inlineButtonBackgroundIdleOpacity: "0.16",
      inlineButtonBackgroundHoverOrFocusOpacity: "0.32",
      inlineButtonBackgroundActiveOpacity: "0.5",
    },
  },
  defaultProps: {
    wrap: false,
  },
});
