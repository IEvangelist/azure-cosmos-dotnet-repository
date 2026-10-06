// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

internal static class InMemoryStorage
{
    private static ConcurrentDictionary<Type, ConcurrentDictionary<string, string>?> TypeToConcurrentDictionary { get; } = new();

    internal static string GetKey(string id, string partitionKey) =>
        $"{partitionKey.Length}:{partitionKey}{id}";

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
        => GetDictionary(typeof(TItem));

    internal static Dictionary<Type, Dictionary<string, string>> Snapshot(IEnumerable<Type> itemTypes) =>
        itemTypes
            .Distinct()
            .ToDictionary(
                itemType => itemType,
                itemType => GetDictionary(itemType).ToDictionary(entry => entry.Key, entry => entry.Value));

    internal static void Restore(IReadOnlyDictionary<Type, Dictionary<string, string>> snapshot)
    {
        foreach (KeyValuePair<Type, Dictionary<string, string>> snapshotEntry in snapshot)
        {
            ConcurrentDictionary<string, string> items = GetDictionary(snapshotEntry.Key);
            items.Clear();

            foreach (KeyValuePair<string, string> itemEntry in snapshotEntry.Value)
            {
                items[itemEntry.Key] = itemEntry.Value;
            }
        }
    }

    private static ConcurrentDictionary<string, string> GetDictionary(Type itemType) =>
        TypeToConcurrentDictionary.GetOrAdd(itemType, static _ => new ConcurrentDictionary<string, string>())!;
}
