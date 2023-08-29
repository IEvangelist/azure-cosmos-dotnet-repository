// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Extensions;

public static class ConcurrentDictionaryExtensions
{
    public static bool TryAddAsJson<TItem>(this ConcurrentDictionary<string, string> items, string id, TItem item) => items.TryAdd(id, JsonConvert.SerializeObject(item));
}