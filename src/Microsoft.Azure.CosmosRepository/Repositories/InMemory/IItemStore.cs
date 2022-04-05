// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal interface IItemStore<TItem> where TItem : IItem
    {
        ValueTask<TItem?> WriteAsync(TItem item, string id, string? partitionKey = null, bool upsert = false);
        ValueTask<TItem?> ReadAsync(string id, string? partitionKey = null);
    }
}