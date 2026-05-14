// @ts-check
import { defineEcConfig } from "astro-expressive-code";

export default defineEcConfig({
  // IMPORTANT: github-light is first so it becomes the unconditional default
  // (emitted without a selector prefix). github-dark only applies when
  // `html.dark` is present, giving us reliable theme switching.
  themes: ["github-light", "github-dark"],
  // EC injects each theme selector as: `${selector} :root, ${selector} .expressive-code, .expressive-code${selector}`.
  // Returning `.dark` produces `:root.dark .expressive-code, .expressive-code.dark` — both valid.
  // (We add the `.dark` class on the <html> element, which is the :root.)
  themeCssSelector: (theme) =>
    theme.name === "github-dark" ? ".dark" : false,
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
