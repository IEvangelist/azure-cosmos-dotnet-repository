// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.InMemory.Exceptions;
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
        private const string DefaultPartitionKey = "partitionKey";
        private readonly string _itemPartitionKey;

        public ItemStore(
            IItemStoreWriterStrategy<TItem> writerStrategy,
            IItemStoreReaderStrategy<TItem> readerStrategy,
            IOptionsMonitor<RepositoryOptions> optionsMonitor
            )
        {
            _writerStrategy = writerStrategy;
            _readerStrategy = readerStrategy;
            _optionsMonitor = optionsMonitor;
            _itemPartitionKey =
                _optionsMonitor.CurrentValue.GetContainerOptions<TItem>()?.PartitionKey?.Replace("/", string.Empty) ??
                DefaultPartitionKey;
        }

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, JObject>> _itemStore = new();

        public async ValueTask<TItem?> WriteAsync(TItem item, string id, string? partitionKey = null, CancellationToken cancellationToken = default)
        {
            partitionKey ??= id;

            ConcurrentDictionary<string, JObject> partitionStore = new ();
            if (_itemStore.TryAdd(partitionKey, partitionStore) is false)
            {
                partitionStore = _itemStore[partitionKey];
            }

            JObject itemJObject = await _writerStrategy.TransformAsync(item, id, partitionKey);

            if (partitionStore.TryAdd(id, itemJObject) is false)
            {
                throw CosmosExceptionHelpers.Conflict();
            }

            return _optionsMonitor.CurrentValue.OptimizeBandwidth ?
                default :
                await ReadAsync(id, partitionKey) ??
                throw new InvalidOperationException("Failed to read back the item");
        }

        public async ValueTask<TItem> UpsertAsync(TItem updatedItem, bool ignoreEtag, CancellationToken cancellationToken = default)
        {
            string partitionKey = updatedItem.PartitionKey;
            string id = updatedItem.Id;

            if (_itemStore.TryGetValue(partitionKey, out ConcurrentDictionary<string, JObject>? partitionStore) is false)
            {
                return await WriteAsync(updatedItem, id, partitionKey, cancellationToken) ?? updatedItem;
            }

            if (partitionStore.TryGetValue(id, out JObject existingItemJObject) is false)
            {
                return await WriteAsync(updatedItem, id, partitionKey, cancellationToken) ?? updatedItem;
            }

            TItem existingItem = await _readerStrategy.TransformAsync(existingItemJObject, cancellationToken);

            JObject updatedItemJObject = await _writerStrategy.TransformAsync(
                updatedItem,
                id,
                partitionKey,
                cancellationToken);

            if (typeof(IItemWithEtag).IsAssignableFrom(typeof(TItem)) &&
                ignoreEtag is false)
            {
                if ((updatedItem as IItemWithEtag)!.Etag != (existingItem as IItemWithEtag)!.Etag)
                {
                    throw CosmosExceptionHelpers.MismatchedEtags();
                }
            }

            partitionStore[id] = updatedItemJObject;

            return _optionsMonitor.CurrentValue.OptimizeBandwidth ?
                updatedItem :
                await ReadAsync(id, partitionKey, cancellationToken) ??
                    throw new UnexpectedFailedReadFromItemStoreException();
        }

        public async ValueTask<TItem?> ReadAsync(string id, string? partitionKey = null, CancellationToken cancellationToken = default)
        {
            partitionKey ??= id;
            if (_itemStore.ContainsKey(partitionKey) &&
                _itemStore[partitionKey].TryGetValue(id, out JObject? item))
            {
                return await _readerStrategy.TransformAsync(item, cancellationToken);
            }

            return default;
        }

        public IAsyncEnumerable<TItem> ReadAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default) =>
            _itemStore
                .Values
                .ToAsyncEnumerable()
                .SelectMany(x => x.Values.ToAsyncEnumerable())
                .SelectAwait(async x => await _readerStrategy.TransformAsync(x, cancellationToken))
                .Where(predicate.Compile());

        public async IAsyncEnumerable<TItem> ReadPartitionAsync(string partitionKey, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (_itemStore.TryGetValue(partitionKey, out ConcurrentDictionary<string, JObject>? partitionStore) is false)
            {
                throw CosmosExceptionHelpers.NotFound();
            }

            foreach (JObject itemJObject in partitionStore.Values)
            {
                yield return await _readerStrategy.TransformAsync(itemJObject, cancellationToken);
            }
        }

        public ValueTask DeleteAsync(
            string id,
            string? partitionKey = null,
            CancellationToken cancellationToken = default)
        {
            partitionKey ??= id;
            if (_itemStore.ContainsKey(partitionKey) is false)
            {
                throw CosmosExceptionHelpers.NotFound();
            }

            if (_itemStore[partitionKey].TryRemove(id, out _) is false)
            {
                throw CosmosExceptionHelpers.NotFound();
            }

            return new ValueTask();
        }
    }
}