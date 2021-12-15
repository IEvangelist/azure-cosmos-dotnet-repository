// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosRepositoryTests.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        public static bool TryAddAsJson<TItem>(this ConcurrentDictionary<string, string> items, string id, TItem item)
        {
            return items.TryAdd(id, JsonConvert.SerializeObject(item));
        }
    }
}