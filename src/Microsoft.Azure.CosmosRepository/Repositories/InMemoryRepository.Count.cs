// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    /// <inheritdoc/>
    public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return InMemoryStorage.GetValues<TItem>()
            .Select(DeserializeItem)
            .Count(item => item?.Type == typeof(TItem).Name);
    }

    /// <inheritdoc/>
    public async ValueTask<int> CountAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return InMemoryStorage.GetValues<TItem>()
            .Select(DeserializeItem)
            .Count(predicate.Compose(
                item => item.Type == typeof(TItem).Name, Expression.AndAlso).Compile());
    }
}