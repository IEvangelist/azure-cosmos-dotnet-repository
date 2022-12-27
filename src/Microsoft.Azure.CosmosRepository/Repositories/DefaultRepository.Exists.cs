// Copyright (c) IEvangelist. All rights reserved.
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
        ExistsAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<bool> ExistsAsync(
        string id,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await _containerProvider.GetContainerAsync().ConfigureAwait(false);

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
        Container container =
            await _containerProvider.GetContainerAsync().ConfigureAwait(false);

        IQueryable<TItem> query =
            container.GetItemLinqQueryable<TItem>()
                .Where(_repositoryExpressionProvider.Build(predicate));

        TryLogDebugDetails(_logger, () => $"Read: {query}");

        var count = await _cosmosQueryableProcessor.CountAsync(query, cancellationToken);
        return count > 0;
    }
}
