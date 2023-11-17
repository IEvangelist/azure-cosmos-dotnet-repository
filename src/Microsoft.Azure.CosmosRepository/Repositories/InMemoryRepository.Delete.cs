// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    /// <inheritdoc/>
    public ValueTask DeleteAsync(TItem value, CancellationToken cancellationToken = default) =>
        DeleteAsync(value.Id, value.PartitionKey, cancellationToken);

    /// <inheritdoc/>
    public ValueTask DeleteAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default) =>
        DeleteAsync(id, new PartitionKey(partitionKeyValue), cancellationToken);

    /// <inheritdoc/>
    public async ValueTask DeleteAsync(
        string id,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        if (partitionKey == default)
        {
            partitionKey = new PartitionKey(id);
        }

        TItem? item = InMemoryStorage
            .GetValues<TItem>()
            .Select(DeserializeItem)
            .FirstOrDefault(i => i.Id == id && new PartitionKey(i.PartitionKey) == partitionKey);

        if (item is null)
        {
            NotFound();
        }


        InMemoryStorage.GetDictionary<TItem>().TryRemove(item!.Id, out _);
    }
}