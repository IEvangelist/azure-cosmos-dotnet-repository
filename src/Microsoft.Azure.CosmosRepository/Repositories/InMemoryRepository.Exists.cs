// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    /// <inheritdoc/>
    public ValueTask<bool> ExistsAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default)
        => ExistsAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<bool> ExistsAsync(
        string id,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
#if NET7_0_OR_GREATER
        await ValueTask.CompletedTask;
#else
        await Task.CompletedTask;
#endif
        return InMemoryStorage
            .GetValues<TItem>()
            .Select(DeserializeItem)
            .FirstOrDefault(i => i.Id == id && new PartitionKey(i.PartitionKey) == partitionKey) is not null;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> ExistsAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
#if NET7_0_OR_GREATER
        await ValueTask.CompletedTask;
#else
        await Task.CompletedTask;
#endif
        return InMemoryStorage
            .GetValues<TItem>().Select(DeserializeItem).Any(predicate.Compose(
            item => item.Type == typeof(TItem).Name, Expression.AndAlso).Compile());
    }
}