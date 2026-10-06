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

        ConcurrentDictionary<string, string> items = InMemoryStorage.GetDictionary<TItem>();
        string? storageKey = null;

        foreach (KeyValuePair<string, string> entry in items)
        {
            TItem item = DeserializeItem(entry.Value);

            if (item.Id == id && new PartitionKey(item.PartitionKey) == partitionKey)
            {
                storageKey = entry.Key;
                break;
            }
        }

        if (storageKey is null)
        {
            NotFound();
        }

        items.TryRemove(storageKey!, out _);
    }
}
