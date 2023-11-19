// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using System.Net;
using Aspire.Microsoft.Azure.CosmosRepository.Containers;
using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Aspire.Microsoft.Azure.CosmosRepository.Internals.Repository;

public class DefaultRepository(
    IItemContainerProvider containerProvider,
    IItemConfiguration itemConfiguration) : IRepository
{
    public ValueTask<TItem> PointReadAsync<TItem>(
        string id,
        CancellationToken cancellationToken = default) where TItem : class, IItem =>
        InternalPointReadAsync<TItem>(
            id,
            cancellationToken: cancellationToken);

    public ValueTask<TItem?> TryPointReadAsync<TItem>(
        string id,
        CancellationToken cancellationToken = default) where TItem : class, IItem =>
        InternalTryPointReadAsync<TItem>(
            id,
            cancellationToken: cancellationToken);

    public ValueTask<TItem> PointReadAsync<TItem>(
        string id,
        string partitionKey,
        CancellationToken cancellationToken = default) where TItem : class, IItem =>
        InternalPointReadAsync<TItem>(
            id,
            cancellationToken: cancellationToken);

    public ValueTask<TItem?> TryPointReadAsync<TItem>(
        string id,
        string partitionKey,
        CancellationToken cancellationToken = default) where TItem : class, IItem =>
        InternalTryPointReadAsync<TItem>(
            id,
            partitionKey,
            cancellationToken);

    public async ValueTask<IEnumerable<TItem>> QueryLogicalPartitionAsync<TItem>(
        Expression<Func<TItem, bool>> predicate,
        string partitionKey,
        string? queryName = null,
        CancellationToken cancellationToken = default) where TItem : class, IItem
    {
        Container container = await containerProvider.GetContainerAsync<TItem>(cancellationToken);
        ICosmosItemConfiguration<TItem> configuration = itemConfiguration.For<TItem>();

        IQueryable<TItem> query = container
            .GetItemLinqQueryable<TItem>()
            .Where(configuration.LogicalPartitionQuery(partitionKey))
            .Where(predicate);

        (IEnumerable<TItem> items, _) = await IterateAsync(
            query,
            cancellationToken);

        return items;
    }

    private async ValueTask<TItem?> InternalTryPointReadAsync<TItem>(
        string id,
        string? partitionKey = null,
        CancellationToken cancellationToken = default) where TItem : class, IItem
    {
        try
        {
            return await InternalPointReadAsync<TItem>(
                id,
                partitionKey,
                cancellationToken);
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    private async ValueTask<TItem> InternalPointReadAsync<TItem>(
        string id,
        string? partitionKey = null,
        CancellationToken cancellationToken = default) where TItem : class, IItem
    {
        Container container = await containerProvider.GetContainerAsync<TItem>(cancellationToken);
        PartitionKey cosmosPk = partitionKey is not null ? new PartitionKey(partitionKey) : new PartitionKey(id);

        return await container.ReadItemAsync<TItem>(
            id,
            cosmosPk,
            cancellationToken: cancellationToken);
    }

    private static async ValueTask<(IEnumerable<TItem> items, double charge)> IterateAsync<TItem>(IQueryable<TItem> queryable, CancellationToken cancellationToken = default) where TItem : IItem
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

            results.AddRange(feedResponse.Resource);
        }

        return (results, charge);
    }
}