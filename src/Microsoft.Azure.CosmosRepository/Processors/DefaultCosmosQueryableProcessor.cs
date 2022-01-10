// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Microsoft.Azure.CosmosRepository.Processors
{
    class DefaultCosmosQueryableProcessor : ICosmosQueryableProcessor
    {
        public async ValueTask<IEnumerable<TItem>> IterateAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem
        {
            using FeedIterator<TItem> iterator = queryable.ToFeedIterator();

            List<TItem> results = new();
            while (iterator.HasMoreResults)
            {
                foreach (TItem result in await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    results.Add(result);
                }
            }

            return results;
        }

        public async ValueTask<int> CountAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem =>
            await queryable.CountAsync(cancellationToken);

        public async ValueTask<IEnumerable<TItem>> IterateAsync<TItem>(Container container, QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default) where TItem : IItem
        {
            using FeedIterator<TItem> queryIterator = container.GetItemQueryIterator<TItem>(queryDefinition);

            List<TItem> results = new();
            while (queryIterator.HasMoreResults)
            {
                FeedResponse<TItem> response = await queryIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
                results.AddRange(response.Resource);
            }

            return results;
        }
    }
}