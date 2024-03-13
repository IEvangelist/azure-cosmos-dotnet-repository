// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal sealed partial class DefaultRepository<TItem>
{
    /// <inheritdoc/>
    public async ValueTask<TItem?> TryGetAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetAsync(id, partitionKeyValue, cancellationToken);
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            logger.LogItemNotFoundHandled<TItem>(id, partitionKeyValue ?? id, e);
            return default;
        }
    }

    //TODO: Write doc
    public async ValueTask<TItem?> TryGetAsync(
        string id,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetAsync(id, partitionKey, cancellationToken);
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            logger.LogItemNotFoundHandled<TItem>(id, partitionKey.ToString() ?? id, e);
            return default;
        }
    }

    //TODO: Write doc
    public async ValueTask<TItem?> TryGetAsync(
        string id,
        IEnumerable<string> partitionKeyValues,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetAsync(id, partitionKeyValues, cancellationToken);
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            logger.LogItemNotFoundHandled<TItem>(id, partitionKeyValues ?? [id], e);
            return default;
        }
    }

    /// <inheritdoc/>
    public ValueTask<TItem> GetAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

    //TODO: Write doc
    public ValueTask<TItem> GetAsync(
        string id,
        IEnumerable<string> partitionKeyValues,
        CancellationToken cancellationToken = default) =>
        GetAsync(id, BuildPartitionKey(partitionKeyValues), cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<TItem> GetAsync(
        string id,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await containerProvider.GetContainerAsync().ConfigureAwait(false);

        if (partitionKey == default)
        {
            partitionKey = new PartitionKey(id);
        }

        logger.LogPointReadStarted<TItem>(id, partitionKey.ToString());

        ItemResponse<TItem> response =
            await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        TItem item = response.Resource;

        logger.LogPointReadExecuted<TItem>(response.RequestCharge);
        logger.LogItemRead(item);

        return repositoryExpressionProvider.CheckItem(item);
    }

    //TODO: Write doc
    public async ValueTask<IEnumerable<TItem>> GetAsync(
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await containerProvider.GetContainerAsync().ConfigureAwait(false);

        IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>(
            requestOptions: new QueryRequestOptions() { PartitionKey = partitionKey },
            linqSerializerOptions: optionsMonitor.CurrentValue.SerializationOptions);

        logger.LogQueryConstructed(query);

        (IEnumerable<TItem> items, var charge) =
            await cosmosQueryableProcessor.IterateAsync(query, cancellationToken);

        logger.LogQueryExecuted(query, charge);

        return items;
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<TItem>> GetAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync(predicate, default, cancellationToken);
    }

    //TODO: Write doc
    public async ValueTask<IEnumerable<TItem>> GetAsync(
        PartitionKey partitionKey,
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync(predicate, partitionKey, cancellationToken);
    }

    private async ValueTask<IEnumerable<TItem>> GetAsync(
        Expression<Func<TItem, bool>> predicate,
        PartitionKey partitionKey = default,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await containerProvider.GetContainerAsync().ConfigureAwait(false);

        var requestOptions = new QueryRequestOptions();

        if (partitionKey != default)
        {
            requestOptions.PartitionKey = partitionKey;
        }

        IQueryable<TItem> query =
            container.GetItemLinqQueryable<TItem>(
                    requestOptions: requestOptions,
                    linqSerializerOptions: optionsMonitor.CurrentValue.SerializationOptions)
                .Where(repositoryExpressionProvider.Build(predicate));

        logger.LogQueryConstructed(query);

        (IEnumerable<TItem> items, var charge) =
            await cosmosQueryableProcessor.IterateAsync(query, cancellationToken);

        logger.LogQueryExecuted(query, charge);

        return items;
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<TItem>> GetByQueryAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await containerProvider.GetContainerAsync().ConfigureAwait(false);

        TryLogDebugDetails(logger, () => $"Read {query}");

        QueryDefinition queryDefinition = new(query);
        return await cosmosQueryableProcessor.IterateAsync<TItem>(container, queryDefinition, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<TItem>> GetByQueryAsync(
        QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await containerProvider.GetContainerAsync().ConfigureAwait(false);

        TryLogDebugDetails(logger, () => $"Read {queryDefinition.QueryText}");

        return await cosmosQueryableProcessor.IterateAsync<TItem>(container, queryDefinition, cancellationToken);
    }
}
