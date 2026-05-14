import * as React from "react";
import { Search } from "lucide-react";

import { Button } from "@/components/ui/button";
import { withBase } from "@/lib/utils";

declare global {
  interface Window {
    __pagefindLoading?: Promise<void>;
    PagefindUI?: new (options: Record<string, unknown>) => unknown;
  }
}

const DIALOG_ID = "pagefind-search-dialog";
const CONTAINER_ID = "pagefind-search-mount";

async function ensurePagefind() {
  if (typeof window === "undefined") return;
  if (window.PagefindUI) return;
  if (window.__pagefindLoading) {
    await window.__pagefindLoading;
    return;
  }
  window.__pagefindLoading = (async () => {
    const cssHref = withBase("/pagefind/pagefind-ui.css");
    if (!document.querySelector(`link[href="${cssHref}"]`)) {
      const link = document.createElement("link");
      link.rel = "stylesheet";
      link.href = cssHref;
      document.head.appendChild(link);
    }
    const scriptSrc = withBase("/pagefind/pagefind-ui.js");
    await new Promise<void>((resolve, reject) => {
      const script = document.createElement("script");
      script.src = scriptSrc;
      script.onload = () => resolve();
      script.onerror = () => reject(new Error("Failed to load Pagefind UI"));
      document.head.appendChild(script);
    });
  })();
  await window.__pagefindLoading;
}

export function SearchTrigger() {
  const dialogRef = React.useRef<HTMLDialogElement | null>(null);
  const [mounted, setMounted] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  const open = React.useCallback(async () => {
    const dialog = dialogRef.current;
    if (!dialog) return;
    if (!dialog.open) {
      try {
        dialog.showModal();
      } catch {
        dialog.show();
      }
    }
    if (!mounted) {
      try {
        await ensurePagefind();
        if (window.PagefindUI) {
          new window.PagefindUI({
            element: `#${CONTAINER_ID}`,
            showSubResults: true,
            resetStyles: false,
          });
          setMounted(true);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : "Search unavailable");
      }
      // Move focus into the search input once the UI mounts.
      requestAnimationFrame(() => {
        const input = dialog.querySelector<HTMLInputElement>(
          ".pagefind-ui__search-input",
        );
        input?.focus();
      });
    }
  }, [mounted]);

  const close = React.useCallback(() => {
    dialogRef.current?.close();
  }, []);

  React.useEffect(() => {
    function onKey(event: KeyboardEvent) {
      if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === "k") {
        event.preventDefault();
        open();
      } else if (event.key === "Escape") {
        if (dialogRef.current?.open) close();
      }
    }
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [open, close]);

  return (
    <>
      <Button
        type="button"
        variant="outline"
        size="sm"
        className="text-muted-foreground hidden h-9 max-w-xs flex-1 justify-between gap-2 text-sm md:flex"
        onClick={open}
        aria-haspopup="dialog"
        aria-controls={DIALOG_ID}
      >
        <span className="inline-flex items-center gap-2">
          <Search className="size-4" aria-hidden="true" />
          Search docs…
        </span>
        <kbd className="bg-muted text-muted-foreground rounded border border-border px-1.5 py-0.5 font-mono text-xs">
          ⌘K
        </kbd>
      </Button>
      <Button
        type="button"
        variant="ghost"
        size="icon"
        className="md:hidden"
        onClick={open}
        aria-label="Open search"
        aria-haspopup="dialog"
        aria-controls={DIALOG_ID}
      >
        <Search aria-hidden="true" />
      </Button>
      <dialog
        id={DIALOG_ID}
        ref={dialogRef}
        aria-label="Search documentation"
        className="bg-popover text-popover-foreground fixed top-[10vh] left-1/2 -translate-x-1/2 m-0 w-[min(640px,92vw)] max-w-none rounded-xl border border-border p-0 shadow-2xl backdrop:bg-black/60 backdrop:backdrop-blur-sm"
        onClick={(event) => {
          if (event.target === event.currentTarget) close();
        }}
      >
        <div className="border-b border-border px-4 py-3 text-sm font-medium flex items-center justify-between">
          <span>Search documentation</span>
          <kbd className="bg-muted text-muted-foreground rounded border border-border px-1.5 py-0.5 font-mono text-xs">
            Esc
          </kbd>
        </div>
        <div id={CONTAINER_ID} className="max-h-[60vh] overflow-y-auto p-3" />
        {error ? (
          <div className="px-4 py-3 text-sm text-destructive" role="alert">
            {error}
          </div>
        ) : null}
      </dialog>
    </>
  );
}
