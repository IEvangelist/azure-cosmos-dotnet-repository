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
    internal sealed partial class ItemStore<TItem> : IItemStore<TItem> where TItem : IItem
    {
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
    }
}