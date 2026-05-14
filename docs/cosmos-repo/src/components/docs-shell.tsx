import * as React from "react";
import { GitPullRequestArrow, PanelLeft } from "lucide-react";

import { AppSidebar } from "@/components/app-sidebar";
import { ThemeToggle } from "@/components/theme-toggle";
import { SearchTrigger } from "@/components/search-trigger";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import {
  SidebarInset,
  SidebarProvider,
  SidebarTrigger,
} from "@/components/ui/sidebar";
import { TooltipProvider } from "@/components/ui/tooltip";
import { withBase } from "@/lib/utils";

export type Crumb = {
  title: string;
  href?: string;
};

export type DocsShellProps = {
  currentPath: string;
  crumbs?: Crumb[];
  editUrl?: string;
  children: React.ReactNode;
};

export function DocsShell({
  currentPath,
  crumbs = [],
  editUrl,
  children,
}: DocsShellProps) {
  return (
    <TooltipProvider delayDuration={300}>
      <SidebarProvider>
        <AppSidebar currentPath={currentPath} />
        <SidebarInset>
          <header className="bg-background/80 supports-[backdrop-filter]:bg-background/60 sticky top-0 z-30 flex h-14 shrink-0 items-center gap-2 border-b border-border backdrop-blur transition-[width,height] ease-linear">
            <div className="flex w-full items-center gap-2 px-4">
              <SidebarTrigger
                className="-ml-1"
                aria-label="Toggle navigation sidebar"
              >
                <PanelLeft aria-hidden="true" />
              </SidebarTrigger>
              <Separator
                orientation="vertical"
                className="mr-2 data-vertical:h-4 data-vertical:self-auto"
              />
              <Breadcrumb className="min-w-0 flex-1">
                <BreadcrumbList>
                  <BreadcrumbItem>
                    <BreadcrumbLink href={withBase("/")}>Home</BreadcrumbLink>
                  </BreadcrumbItem>
                  {crumbs.map((c, i) => {
                    const isLast = i === crumbs.length - 1;
                    return (
                      <React.Fragment key={`${c.title}-${i}`}>
                        <BreadcrumbSeparator />
                        <BreadcrumbItem>
                          {isLast || !c.href ? (
                            <BreadcrumbPage className="truncate">
                              {c.title}
                            </BreadcrumbPage>
                          ) : (
                            <BreadcrumbLink href={c.href}>
                              {c.title}
                            </BreadcrumbLink>
                          )}
                        </BreadcrumbItem>
                      </React.Fragment>
                    );
                  })}
                </BreadcrumbList>
              </Breadcrumb>
              <div className="ml-auto flex items-center gap-1">
                <SearchTrigger />
                {editUrl ? (
                  <Button asChild variant="ghost" size="sm" className="hidden md:inline-flex">
                    <a
                      href={editUrl}
                      target="_blank"
                      rel="noreferrer"
                      aria-label="Edit this page on GitHub"
                    >
                      <GitPullRequestArrow
                        className="mr-1 size-4"
                        aria-hidden="true"
                      />
                      Edit
                    </a>
                  </Button>
                ) : null}
                <ThemeToggle />
              </div>
            </div>
          </header>
          <main id="main" className="flex flex-1 flex-col px-4 py-6 md:px-8">
            <div className="prose-docs mx-auto w-full max-w-3xl">{children}</div>
          </main>
        </SidebarInset>
      </SidebarProvider>
    </TooltipProvider>
  );
}
