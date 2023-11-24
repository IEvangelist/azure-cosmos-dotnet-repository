// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    public async ValueTask<TItem?> TryGetAsync(string id, string? partitionKeyValue = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetAsync(id, partitionKeyValue, cancellationToken);
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    /// <inheritdoc/>
    public ValueTask<TItem> GetAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default)
        => GetAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<TItem> GetAsync(string id, PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
#if NET7_0_OR_GREATER
        await ValueTask.CompletedTask;
#else
        await Task.CompletedTask;
#endif

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

        TItem? toReturn = item is { Type.Length: 0 } || item?.Type == typeof(TItem).Name ? item : default;
        return toReturn!;
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
#if NET7_0_OR_GREATER
        await ValueTask.CompletedTask;
#else
        await Task.CompletedTask;
#endif
        return InMemoryStorage.GetValues<TItem>().Select(DeserializeItem).Where(predicate.Compose(
            item => item.Type == typeof(TItem).Name, Expression.AndAlso).Compile());
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<TItem>> GetByQueryAsync(string query,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public ValueTask<IEnumerable<TItem>> GetByQueryAsync(QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();
}