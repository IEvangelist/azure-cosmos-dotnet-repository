import * as React from "react";
import {
  Activity,
  BookOpen,
  Boxes,
  ChevronRight,
  Layers,
  Rocket,
  Search,
  SlidersHorizontal,
  type LucideIcon,
} from "lucide-react";

import { docsNav, type NavItem } from "@/lib/nav";
import { withBase } from "@/lib/utils";
import { cn } from "@/lib/utils";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
  SidebarMenuSubButton,
  SidebarMenuSubItem,
  SidebarRail,
} from "@/components/ui/sidebar";

const ICONS: Record<string, LucideIcon> = {
  Rocket,
  Boxes,
  Layers,
  Search,
  Activity,
  SlidersHorizontal,
  BookOpen,
};

function isPathActive(item: NavItem, currentPath: string): boolean {
  const normalize = (p: string) => p.replace(/\/$/, "");
  const cur = normalize(currentPath);
  if (normalize(item.url) === cur) return true;
  if (item.items?.some((c) => normalize(c.url) === cur)) return true;
  return false;
}

export type AppSidebarProps = React.ComponentProps<typeof Sidebar> & {
  currentPath: string;
};

export function AppSidebar({ currentPath, ...props }: AppSidebarProps) {
  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" asChild>
              <a href={withBase("/")} aria-label="Cosmos Repository home">
                <div
                  className="flex aspect-square size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground"
                  aria-hidden="true"
                >
                  <Boxes className="size-4" />
                </div>
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-semibold">
                    Cosmos Repository
                  </span>
                  <span className="truncate text-xs text-muted-foreground">
                    .NET SDK · Documentation
                  </span>
                </div>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Documentation</SidebarGroupLabel>
          <SidebarMenu>
            {docsNav.map((section) => {
              const Icon = section.icon ? ICONS[section.icon] : undefined;
              const active = isPathActive(section, currentPath);
              const hasChildren = (section.items?.length ?? 0) > 1;
              if (!hasChildren) {
                return (
                  <SidebarMenuItem key={section.title}>
                    <SidebarMenuButton
                      tooltip={section.title}
                      isActive={active}
                      asChild
                    >
                      <a href={withBase(section.url)}>
                        {Icon ? <Icon aria-hidden="true" /> : null}
                        <span>{section.title}</span>
                      </a>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                );
              }
              return (
                <Collapsible
                  key={section.title}
                  asChild
                  defaultOpen={active}
                  className="group/collapsible"
                >
                  <SidebarMenuItem>
                    <CollapsibleTrigger asChild>
                      <SidebarMenuButton
                        tooltip={section.title}
                        isActive={active}
                      >
                        {Icon ? <Icon aria-hidden="true" /> : null}
                        <span>{section.title}</span>
                        <ChevronRight
                          className={cn(
                            "ml-auto transition-transform duration-200",
                            "group-data-[state=open]/collapsible:rotate-90",
                          )}
                          aria-hidden="true"
                        />
                      </SidebarMenuButton>
                    </CollapsibleTrigger>
                    <CollapsibleContent>
                      <SidebarMenuSub>
                        {section.items!.map((sub) => {
                          const subActive =
                            currentPath.replace(/\/$/, "") ===
                            sub.url.replace(/\/$/, "");
                          return (
                            <SidebarMenuSubItem key={sub.title}>
                              <SidebarMenuSubButton
                                asChild
                                isActive={subActive}
                              >
                                <a
                                  href={withBase(sub.url)}
                                  aria-current={subActive ? "page" : undefined}
                                >
                                  <span>{sub.title}</span>
                                </a>
                              </SidebarMenuSubButton>
                            </SidebarMenuSubItem>
                          );
                        })}
                      </SidebarMenuSub>
                    </CollapsibleContent>
                  </SidebarMenuItem>
                </Collapsible>
              );
            })}
          </SidebarMenu>
        </SidebarGroup>
      </SidebarContent>
      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton asChild tooltip="GitHub repository">
              <a
                href="https://github.com/IEvangelist/azure-cosmos-dotnet-repository"
                rel="noreferrer"
                target="_blank"
              >
                <BookOpen aria-hidden="true" />
                <span>GitHub</span>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
          <SidebarMenuItem>
            <SidebarMenuButton asChild tooltip="NuGet package">
              <a
                href="https://www.nuget.org/packages/IEvangelist.Azure.CosmosRepository"
                rel="noreferrer"
                target="_blank"
              >
                <Rocket aria-hidden="true" />
                <span>NuGet</span>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
      <SidebarRail />
    </Sidebar>
  );
}
