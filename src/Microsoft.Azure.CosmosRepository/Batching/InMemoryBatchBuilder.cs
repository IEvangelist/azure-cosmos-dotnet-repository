namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// In-memory batch builder that executes queued operations sequentially.
/// </summary>
/// <remarks>The in-memory implementation is not atomic.</remarks>
internal sealed class InMemoryBatchBuilder(string partitionKey) : IBatchBuilder
{
    private readonly string _partitionKey = partitionKey;
    private readonly List<Func<CancellationToken, ValueTask>> _operations = [];

    public IBatchBuilder CreateItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        _operations.Add(cancellationToken => CreateAsync(item, cancellationToken));

        return this;
    }

    public IBatchBuilder ReplaceItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        _operations.Add(cancellationToken => UpsertAsync(item, cancellationToken));

        return this;
    }

    public IBatchBuilder UpsertItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        _operations.Add(cancellationToken => UpsertAsync(item, cancellationToken));

        return this;
    }

    public IBatchBuilder DeleteItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        _operations.Add(cancellationToken => DeleteAsync<TItem>(item.Id, cancellationToken));

        return this;
    }

    public IBatchBuilder DeleteItem<TItem>(string id) where TItem : IItem
    {
        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        _operations.Add(cancellationToken => DeleteAsync<TItem>(id, cancellationToken));

        return this;
    }

    public async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_operations.Count == 0)
        {
            throw new ArgumentException(
                "Unable to perform batch operation with no items",
                nameof(_operations));
        }

        foreach (Func<CancellationToken, ValueTask> operation in _operations)
        {
            await operation(cancellationToken).ConfigureAwait(false);
        }
    }

    private void ValidatePartitionKey(IItem item)
    {
        if (!string.Equals(item.PartitionKey, _partitionKey, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"The item partition key '{item.PartitionKey}' does not match the batch partition key '{_partitionKey}'.",
                nameof(item));
        }
    }

    private static async ValueTask CreateAsync<TItem>(TItem item, CancellationToken cancellationToken) where TItem : IItem =>
        await new InMemoryRepository<TItem>().CreateAsync(item, cancellationToken).ConfigureAwait(false);

    private static async ValueTask UpsertAsync<TItem>(TItem item, CancellationToken cancellationToken) where TItem : IItem =>
        await new InMemoryRepository<TItem>().UpdateAsync(item, cancellationToken: cancellationToken).ConfigureAwait(false);

    private async ValueTask DeleteAsync<TItem>(string id, CancellationToken cancellationToken) where TItem : IItem =>
        await new InMemoryRepository<TItem>().DeleteAsync(id, _partitionKey, cancellationToken).ConfigureAwait(false);
}
