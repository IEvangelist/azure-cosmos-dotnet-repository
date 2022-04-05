// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Extensions;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal interface IItemStoreWriterStrategy<TItem> where TItem : IItem
    {
        ValueTask<JObject> TransformAsync(TItem item, string id, string? partitionKey = null);
    }
}