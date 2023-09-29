// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Processors;

class DefaultCosmosQueryableProcessor : ICosmosQueryableProcessor
{
    public async ValueTask<(IEnumerable<TItem> items, double charge)> IterateAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem
    {
        using var iterator = queryable.ToFeedIterator();

        List<TItem> results = [];
        double charge = 0;

        while (iterator.HasMoreResults)
        {
            FeedResponse<TItem> feedResponse = await iterator
                .ReadNextAsync(cancellationToken)
                .ConfigureAwait(false);

            charge += feedResponse.RequestCharge;

            foreach (TItem result in feedResponse.Resource)
            {
                results.Add(result);
            }
        }

        return (results, charge);
    }

    public async ValueTask<int> CountAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem =>
        await queryable.CountAsync(cancellationToken);

    public async ValueTask<IEnumerable<TItem>> IterateAsync<TItem>(Container container, QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default) where TItem : IItem
    {
        using FeedIterator<TItem> queryIterator = container.GetItemQueryIterator<TItem>(queryDefinition);

        List<TItem> results = [];

        while (queryIterator.HasMoreResults)
        {
            FeedResponse<TItem> response = await queryIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            results.AddRange(response.Resource);
        }

        return results;
    }
}