// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Processors;

interface ICosmosQueryableProcessor
{
    ValueTask<(IEnumerable<TItem> items, double charge)> IterateAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem;

    ValueTask<int> CountAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem;

    ValueTask<IEnumerable<TItem>> IterateAsync<TItem>(Container container, QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default) where TItem : IItem;
}