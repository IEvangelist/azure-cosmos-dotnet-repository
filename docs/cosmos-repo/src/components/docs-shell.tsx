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
        <SidebarInset className="min-w-0">
          <header className="bg-background/80 supports-[backdrop-filter]:bg-background/60 sticky top-0 z-30 flex h-14 shrink-0 items-center gap-2 border-b border-border backdrop-blur transition-[width,height] ease-linear">
            <div className="flex w-full min-w-0 items-center gap-2 px-4">
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
                            <BreadcrumbLink href={withBase(c.href)}>
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
                <Button
                  asChild
                  variant="ghost"
                  size="icon"
                  className="cursor-pointer"
                >
                  <a
                    href="https://github.com/IEvangelist/azure-cosmos-dotnet-repository"
                    target="_blank"
                    rel="noreferrer"
                    aria-label="GitHub repository"
                  >
                    <svg
                      role="img"
                      viewBox="0 0 24 24"
                      xmlns="http://www.w3.org/2000/svg"
                      className="size-4 fill-current"
                      aria-hidden="true"
                    >
                      <path d="M12 .5C5.65.5.5 5.65.5 12c0 5.08 3.29 9.39 7.86 10.91.58.1.79-.25.79-.55v-1.93c-3.2.7-3.88-1.54-3.88-1.54-.52-1.34-1.28-1.69-1.28-1.69-1.05-.72.08-.71.08-.71 1.16.08 1.78 1.2 1.78 1.2 1.04 1.78 2.72 1.27 3.39.97.1-.75.41-1.27.74-1.56-2.55-.29-5.24-1.28-5.24-5.7 0-1.26.45-2.29 1.19-3.1-.12-.29-.52-1.46.11-3.05 0 0 .98-.31 3.2 1.18.93-.26 1.93-.39 2.92-.39s1.99.13 2.92.39c2.22-1.49 3.2-1.18 3.2-1.18.63 1.59.23 2.76.11 3.05.74.81 1.19 1.84 1.19 3.1 0 4.43-2.69 5.41-5.25 5.69.42.36.79 1.07.79 2.15v3.19c0 .31.21.66.8.55C20.21 21.39 23.5 17.08 23.5 12 23.5 5.65 18.35.5 12 .5z" />
                    </svg>
                  </a>
                </Button>
              </div>
            </div>
          </header>
          <main id="main" className="flex min-w-0 flex-1 flex-col px-4 py-6 md:px-8">
            <div className="prose-docs mx-auto w-full min-w-0 max-w-3xl">{children}</div>
          </main>
        </SidebarInset>
      </SidebarProvider>
    </TooltipProvider>
  );
}
