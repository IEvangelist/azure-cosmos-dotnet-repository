// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    /// <inheritdoc/>
    public ValueTask<TItem> CreateAsync(TItem value, CancellationToken cancellationToken = default) =>
        CreateAsync(value, true);

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<TItem>> CreateAsync(IEnumerable<TItem> values,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<TItem> enumerable = values.ToList();

        List<TItem> results = [];

        foreach (TItem value in enumerable)
        {
            TItem item = await CreateAsync(value, false);
            results.Add(item);
        }

        Changes?.Invoke(new ChangeFeedItemArgs<TItem>(results));

        return results;
    }

    private async ValueTask<TItem> CreateAsync(TItem value, bool raiseChanges = false)
    {
        await Task.CompletedTask;

        TItem? item = InMemoryStorage
            .GetValues<TItem>()
            .Select(DeserializeItem)
            .FirstOrDefault(i => i.Id == value.Id && i.PartitionKey == value.PartitionKey);

        if (item is not null)
        {
            Conflict();
        }

        if (value is IItemWithTimeStamps { CreatedTimeUtc: null } valueWithTimestamps)
        {
            valueWithTimestamps.CreatedTimeUtc = DateTime.UtcNow;
        }

        var serialisedValue = InMemoryRepository<TItem>.SerializeItem(value, Guid.NewGuid().ToString(), CurrentTs);

        ConcurrentDictionary<string, string> items = InMemoryStorage.GetDictionary<TItem>();
        items.TryAdd(value.Id, serialisedValue);

        value = DeserializeItem(items[value.Id]);

        if (raiseChanges)
        {
            Changes?.Invoke(new ChangeFeedItemArgs<TItem>(value));
        }

        return value;
    }
}