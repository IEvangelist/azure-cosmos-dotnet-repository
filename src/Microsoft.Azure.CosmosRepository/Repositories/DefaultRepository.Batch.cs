// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class DefaultRepository<TItem>
{
    /// <inheritdoc />
    public async ValueTask UpdateAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        PartitionKey partitionKey = BuildPartitionKey(list);

        await UpdateAsBatchAsync(items, partitionKey, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask UpdateAsBatchAsync(
        IEnumerable<TItem> items,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        Container container = await containerProvider.GetContainerAsync();

        TransactionalBatch batch = container.CreateTransactionalBatch(partitionKey);

        foreach (TItem item in list)
        {
            TransactionalBatchItemRequestOptions options = new();

            if (item is IItemWithEtag itemWithEtag)
            {
                options.IfMatchEtag = itemWithEtag.Etag;
            }

            batch.UpsertItem(item, options);
        }

        using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new BatchOperationException<TItem>(response);
        }
    }

    /// <inheritdoc />
    public async ValueTask CreateAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        PartitionKey partitionKey = BuildPartitionKey(list);

        await CreateAsBatchAsync(items, partitionKey, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask CreateAsBatchAsync(
        IEnumerable<TItem> items,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        Container container = await containerProvider.GetContainerAsync();

        TransactionalBatch batch = container.CreateTransactionalBatch(partitionKey);

        foreach (TItem item in list)
        {
            batch.CreateItem(item);
        }

        using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new BatchOperationException<TItem>(response);
        }
    }

    /// <inheritdoc />
    public async ValueTask DeleteAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        PartitionKey partitionKey = BuildPartitionKey(list);

        await DeleteAsBatchAsync(items, partitionKey, cancellationToken);
      
    }

    /// <inheritdoc />
    public async ValueTask DeleteAsBatchAsync(
        IEnumerable<TItem> items,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        Container container = await containerProvider.GetContainerAsync();

        TransactionalBatch batch = container.CreateTransactionalBatch(partitionKey);

        foreach (TItem item in list)
        {
            batch.DeleteItem(item.Id);
        }

        using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new BatchOperationException<TItem>(response);
        }
    }
}