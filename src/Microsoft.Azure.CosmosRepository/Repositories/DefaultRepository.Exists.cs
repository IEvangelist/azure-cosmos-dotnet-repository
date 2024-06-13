// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal sealed partial class DefaultRepository<TItem>
{
    /// <inheritdoc/>
    public ValueTask<bool> ExistsAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(id, BuildPartitionKey(partitionKeyValue ?? id), cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<bool> ExistsAsync(
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

        try
        {
            _ = await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> ExistsAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(predicate, default, cancellationToken);
    }

    //TODO: Write docs
    public async ValueTask<bool> ExistsAsync(
        Expression<Func<TItem, bool>> predicate,
        PartitionKey partitionKey,
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

        TryLogDebugDetails(logger, () => $"Read: {query}");

        var count = await cosmosQueryableProcessor.CountAsync(query, cancellationToken);
        return count > 0;
    }


    //TODO: Write doc
    public async ValueTask<bool> ExistsAsync(string id, IEnumerable<string> partitionKeyValues, CancellationToken cancellationToken = default)
    {
       return await ExistsAsync(id, BuildPartitionKey(partitionKeyValues, id), cancellationToken);
    }

}
