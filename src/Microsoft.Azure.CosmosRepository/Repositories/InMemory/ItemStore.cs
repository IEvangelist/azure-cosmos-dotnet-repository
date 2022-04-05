// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.InMemory.Reader;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed class ItemStore<TItem> : IItemStore<TItem> where TItem : IItem
    {
        private readonly IItemStoreWriterStrategy<TItem> _writerStrategy;
        private readonly IItemStoreReaderStrategy<TItem> _readerStrategy;
        private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;

        public ItemStore(
            IItemStoreWriterStrategy<TItem> writerStrategy,
            IItemStoreReaderStrategy<TItem> readerStrategy,
            IOptionsMonitor<RepositoryOptions> optionsMonitor
            )
        {
            _writerStrategy = writerStrategy;
            _readerStrategy = readerStrategy;
            _optionsMonitor = optionsMonitor;
        }

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, JObject>> _itemStore = new();

        public async ValueTask<TItem?> WriteAsync(TItem item, string id, string? partitionKey = null, bool upsert = false)
        {
            partitionKey ??= id;

            ConcurrentDictionary<string, JObject> partitionStore = new ();
            if (_itemStore.TryAdd(partitionKey, partitionStore) is false)
            {
                partitionStore = _itemStore[partitionKey];
            }

            JObject itemJObject = await _writerStrategy.TransformAsync(item, id, partitionKey);

            if (upsert)
            {
                partitionStore[id] = itemJObject;
            }
            else
            {
                if (partitionStore.TryAdd(id, itemJObject) is false)
                {
                    throw CosmosExceptionHelpers.Conflict();
                }
            }

            return _optionsMonitor.CurrentValue.OptimizeBandwidth ?
                default :
                await ReadAsync(id, partitionKey) ??
                throw new InvalidOperationException("Failed to read back the item");
        }

        public async ValueTask<TItem?> ReadAsync(string id, string? partitionKey = null)
        {
            partitionKey ??= id;
            if (_itemStore.ContainsKey(partitionKey) &&
                _itemStore[partitionKey].TryGetValue(id, out JObject? item))
            {
                return await _readerStrategy.TransformAsync(item);
            }

            return default;
        }

        public ValueTask DeleteAsync(string id, string? partitionKey = null)
        {
            partitionKey ??= id;
            if (_itemStore.ContainsKey(partitionKey))
            {
                _itemStore[partitionKey].TryRemove(id, out _);
            }

            return new ValueTask();
        }
    }
}