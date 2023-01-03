// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    public async ValueTask CreateAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        foreach (TItem? item in items)
        {
            await CreateAsync(item, cancellationToken);
        }
    }

    public async ValueTask UpdateAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        foreach (TItem? item in items)
        {
            await UpdateAsync(item, cancellationToken: cancellationToken);
        }
    }

    public async ValueTask DeleteAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        foreach (TItem? item in items)
        {
            await DeleteAsync(item, cancellationToken);
        }
    }
}