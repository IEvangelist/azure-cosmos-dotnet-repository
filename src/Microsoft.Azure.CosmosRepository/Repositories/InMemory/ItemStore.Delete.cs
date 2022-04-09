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