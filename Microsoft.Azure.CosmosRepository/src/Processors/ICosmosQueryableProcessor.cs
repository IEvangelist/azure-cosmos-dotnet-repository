// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Processors
{
    interface ICosmosQueryableProcessor
    {
        ValueTask<IEnumerable<TItem>> IterateAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem;

        ValueTask<int> CountAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem;

        ValueTask<IEnumerable<TItem>> IterateAsync<TItem>(Container container, QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default) where TItem : IItem;
    }
}