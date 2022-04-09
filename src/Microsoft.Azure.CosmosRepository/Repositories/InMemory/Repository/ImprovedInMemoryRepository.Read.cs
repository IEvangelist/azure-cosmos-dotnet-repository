// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed partial class ImprovedInMemoryRepository<TItem> : IRepository<TItem> where TItem : IItem
    {
        public async ValueTask<IEnumerable<TItem>> GetAsync(
            Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default) =>
                await _itemStore.ReadAsync(predicate, cancellationToken).ToListAsync(cancellationToken);

        public ValueTask<TItem?> TryGetAsync(
            string id,
            string? partitionKeyValue = null,
            CancellationToken cancellationToken = default) =>
                _itemStore.ReadAsync(id, partitionKeyValue, cancellationToken);

        public async ValueTask<TItem> GetAsync(
            string id,
            string? partitionKeyValue = null,
            CancellationToken cancellationToken = default) =>
                await _itemStore.ReadAsync(id, partitionKeyValue, cancellationToken) ??
                    throw CosmosExceptionHelpers.NotFound();
    }
}