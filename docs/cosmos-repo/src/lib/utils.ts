import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const SITE = "https://ievangelist.github.io";
export const BASE = "/azure-cosmos-dotnet-repository";
export const REPO_URL =
  "https://github.com/IEvangelist/azure-cosmos-dotnet-repository";

/**
 * Build an absolute path with the GitHub Pages base path prefix.
 * Pass in a leading-slash app path; returns a string Astro / browser can navigate to.
 */
export function withBase(path: string): string {
  if (path.startsWith("http://") || path.startsWith("https://")) {
    return path;
  }
  const cleaned = path.startsWith("/") ? path : `/${path}`;
  return `${BASE}${cleaned}`;
}
