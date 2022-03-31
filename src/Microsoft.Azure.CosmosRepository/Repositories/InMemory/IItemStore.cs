// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal interface IItemStore<TItem> where TItem : IItem
    {
        ValueTask<TItem> WriteAsync(TItem item, string id, string? partitionKey = null);
        ValueTask<TItem> ReadAsync(string id, string? partitionKey = null);
    }

    internal class ItemStore<TItem> : IItemStore<TItem> where TItem : IItem
    {
        private readonly IItemStoreWriterStrategy<TItem> _writerStrategy;

        public ItemStore(IItemStoreWriterStrategy<TItem> writerStrategy)
        {
            _writerStrategy = writerStrategy;
        }

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _itemStore = new();

        public async ValueTask<TItem> WriteAsync(TItem item, string id, string? partitionKey = null)
        {
            partitionKey ??= id;
            string json = await _writerStrategy.WriteAsync(item, id, partitionKey);

            if (_itemStore.TryGetValue(partitionKey, out ConcurrentDictionary<string, string>? partitionStore))
            {
                partitionStore[id] = json;
            }
            else
            {
                _itemStore[partitionKey] = new ConcurrentDictionary<string, string>
                {
                    [id] = json
                };
            }

            return await ReadAsync(id, partitionKey);
        }

        public ValueTask<TItem> ReadAsync(string id, string? partitionKey = null)
        {
            throw new NotImplementedException();
        }
    }
}