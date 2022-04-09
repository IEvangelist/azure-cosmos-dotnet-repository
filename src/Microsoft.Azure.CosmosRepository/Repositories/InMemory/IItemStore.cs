// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal interface IItemStore<TItem> where TItem : IItem
    {
        ValueTask<TItem?> WriteAsync(TItem item, string id, string? partitionKey = null, CancellationToken cancellationToken = default);
        ValueTask<TItem> UpsertAsync(TItem updatedItem, bool ignoreEtag, CancellationToken cancellationToken = default);
        ValueTask<TItem?> ReadAsync(string id, string? partitionKey = null, CancellationToken cancellationToken = default);
        ValueTask DeleteAsync(string id, string? partitionKey = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<TItem> ReadAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default);
        IAsyncEnumerable<TItem> ReadPartitionAsync(string partitionKey, CancellationToken cancellationToken = default);
    }
}