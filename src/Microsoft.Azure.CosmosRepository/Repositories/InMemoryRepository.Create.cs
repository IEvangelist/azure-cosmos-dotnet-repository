// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory;

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

        List<TItem> results = new();

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

        TItem? item = Items
            .Values
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

        var serialisedValue = SerializeItem(value, Guid.NewGuid().ToString(), CurrentTs);
        Items.TryAdd(value.Id, serialisedValue);

        value = DeserializeItem(Items[value.Id]);

        if (raiseChanges)
        {
            Changes?.Invoke(new ChangeFeedItemArgs<TItem>(value));
        }

        return value;
    }
}