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

            if (DoEtagsMatch(updatedItem as IItemWithEtag, existingItem as IItemWithEtag, ignoreEtag) is false)
            {
                throw CosmosExceptionHelpers.MismatchedEtags();
            }

            partitionStore[id] = updatedItemJObject;

            return _optionsMonitor.CurrentValue.OptimizeBandwidth ?
                updatedItem :
                await ReadAsync(id, partitionKey, cancellationToken) ??
                throw new UnexpectedFailedReadFromItemStoreException();
        }

        private bool DoEtagsMatch(IItemWithEtag? updatedItem, IItemWithEtag? existingItem, bool ignoreEtag)
        {
            if (updatedItem is null || existingItem is null)
            {
                return true;
            }

            return ignoreEtag || updatedItem.Etag is null || updatedItem.Etag == existingItem.Etag;
        }
    }
}