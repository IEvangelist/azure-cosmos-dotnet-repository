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
    }
}