// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal sealed partial class DefaultRepository<TItem>
{
    /// <inheritdoc/>
    public async ValueTask<IPage<TItem>> PageAsync(
        Expression<Func<TItem, bool>>? predicate = null,
        int pageSize = 25,
        string? continuationToken = null,
        bool returnTotal = false,
        CancellationToken cancellationToken = default)
    {
        return await InternalPageAsync(predicate, default, pageSize, continuationToken, returnTotal, cancellationToken);
    }

    //TODO: Write doc
    public async ValueTask<IPage<TItem>> PageAsync(
        PartitionKey partitionKey,
        Expression<Func<TItem, bool>>? predicate = null,
        int pageSize = 25,
        string? continuationToken = null,
        bool returnTotal = false,
        CancellationToken cancellationToken = default)
    {
        return await InternalPageAsync(predicate, partitionKey, pageSize, continuationToken, returnTotal, cancellationToken);
    }

    private async ValueTask<IPage<TItem>> InternalPageAsync(
        Expression<Func<TItem, bool>>? predicate = null,
        PartitionKey partitionKey = default,
        int pageSize = 25,
        string? continuationToken = null,
        bool returnTotal = false,
        CancellationToken cancellationToken = default)
    {
        Container container = await containerProvider.GetContainerAsync()
            .ConfigureAwait(false);

        QueryRequestOptions options = new()
        {
            MaxItemCount = pageSize
        };

        if (partitionKey != default)
        {
            options.PartitionKey = partitionKey;
        }

        IQueryable<TItem> query = container
            .GetItemLinqQueryable<TItem>(
                requestOptions: options,
                continuationToken: continuationToken,
                linqSerializerOptions: optionsMonitor.CurrentValue.SerializationOptions)
            .Where(repositoryExpressionProvider.Build(
                predicate ??
                repositoryExpressionProvider.Default<TItem>()));

        logger.LogQueryConstructed(query);

        Response<int>? countResponse = null;

        if (returnTotal)
        {
            countResponse = await query.CountAsync(cancellationToken);
        }

        (List<TItem> items, var charge, var resultingContinuationToken) =
            await GetAllItemsAsync(query, pageSize, cancellationToken);

        logger.LogQueryExecuted(query, charge);

        return new Page<TItem>(
            countResponse?.Resource ?? null,
            pageSize,
            items.AsReadOnly(),
            charge + countResponse?.RequestCharge ?? 0,
            resultingContinuationToken);
    }

    public async ValueTask<IPageQueryResult<TItem>> PageAsync(
        PartitionKey partitionKey,
        Expression<Func<TItem, bool>>? predicate = null,
        int pageNumber = 1,
        int pageSize = 25,
        bool returnTotal = false,
        CancellationToken cancellationToken = default)
    {
        return await InternalPageAsync(predicate, partitionKey, pageNumber, pageSize, returnTotal, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<IPageQueryResult<TItem>> PageAsync(
        Expression<Func<TItem, bool>>? predicate = null,
        int pageNumber = 1,
        int pageSize = 25,
        bool returnTotal = false,
        CancellationToken cancellationToken = default)
    {
        return await InternalPageAsync(predicate, default, pageNumber, pageSize, returnTotal, cancellationToken);
    }


    private async ValueTask<IPageQueryResult<TItem>> InternalPageAsync(
        Expression<Func<TItem, bool>>? predicate = null,
        PartitionKey partitionKey = default,
        int pageNumber = 1,
        int pageSize = 25,
        bool returnTotal = false,
        CancellationToken cancellationToken = default)
    {
        Container container = await containerProvider.GetContainerAsync()
            .ConfigureAwait(false);

        var options = new QueryRequestOptions();

        if (partitionKey != default)
        {
            options.PartitionKey = partitionKey;
        }

        IQueryable<TItem> query = container
            .GetItemLinqQueryable<TItem>(
            requestOptions: options,
                linqSerializerOptions: optionsMonitor.CurrentValue.SerializationOptions)
            .Where(repositoryExpressionProvider
                .Build(predicate ?? repositoryExpressionProvider.Default<TItem>()));

        Response<int>? countResponse = null;

        if (returnTotal)
        {
            countResponse = await query.CountAsync(cancellationToken);
        }

        query = query.Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);

        logger.LogQueryConstructed(query);

        (List<TItem> items, var charge, var resultingContinuationToken) =
            await GetAllItemsAsync(query, pageSize, cancellationToken);

        logger.LogQueryExecuted(query, charge);

        return new PageQueryResult<TItem>(
            countResponse?.Resource ?? null,
            pageNumber,
            pageSize,
            items.AsReadOnly(),
            charge + countResponse?.RequestCharge ?? 0,
            resultingContinuationToken /* This was missing, is this correct? */);
    }
}
