// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

internal static class InMemoryStorage
{
    private static ConcurrentDictionary<Type, ConcurrentDictionary<string, string>?> TypeToConcurrentDictionary { get; } = new();

    internal static IEnumerable<string> GetValues<TItem>() where TItem : IItem
    {
        if (TypeToConcurrentDictionary.TryGetValue(typeof(TItem), out ConcurrentDictionary<string, string>? value))
        {
            return value?.Values ?? [];
        }

        TypeToConcurrentDictionary[typeof(TItem)] = new ConcurrentDictionary<string, string>();
        return TypeToConcurrentDictionary[typeof(TItem)]?.Values ?? [];
    }

    internal static ConcurrentDictionary<string, string> GetDictionary<TItem>() where TItem : IItem
    {
        if (TypeToConcurrentDictionary.TryGetValue(typeof(TItem), out ConcurrentDictionary<string, string>? value))
        {
            return value ?? new ConcurrentDictionary<string, string>();
        }

        TypeToConcurrentDictionary[typeof(TItem)] = new ConcurrentDictionary<string, string>();
        return TypeToConcurrentDictionary[typeof(TItem)] ?? new ConcurrentDictionary<string, string>();
    }
}