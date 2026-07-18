namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// In-memory batch builder that executes queued operations sequentially.
/// </summary>
/// <remarks>The in-memory implementation is not atomic.</remarks>
internal sealed class InMemoryBatchBuilder(
    string partitionKey,
    IServiceProvider serviceProvider) : IBatchBuilder
{
    private readonly string _partitionKey = partitionKey;
    private readonly List<Func<CancellationToken, ValueTask>> _operations = [];
    private readonly HashSet<Type> _seenTypes = [];

    public IBatchBuilder CreateItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        EnsureCapacity();
        _seenTypes.Add(typeof(TItem));
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
        EnsureCapacity();
        _seenTypes.Add(typeof(TItem));
        _operations.Add(cancellationToken => ReplaceAsync(item, cancellationToken));

        return this;
    }

    public IBatchBuilder UpsertItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        EnsureCapacity();
        _seenTypes.Add(typeof(TItem));
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
        EnsureCapacity();
        _seenTypes.Add(typeof(TItem));
        _operations.Add(cancellationToken => DeleteAsync<TItem>(item.Id, cancellationToken));

        return this;
    }

    public IBatchBuilder DeleteItem<TItem>(string id) where TItem : IItem
    {
        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        EnsureCapacity();
        _seenTypes.Add(typeof(TItem));
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

        // Mirrors the same-container validation the Cosmos builder performs
        // through ICosmosContainerService.GetContainerAsync(IReadOnlyList<Type>).
        BatchContainerValidation.EnsureSameContainer(
            _seenTypes,
            serviceProvider.GetRequiredService<IOptions<RepositoryOptions>>().Value,
            serviceProvider.GetRequiredService<ICosmosContainerNameProvider>().GetContainerName);

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

    private void EnsureCapacity()
    {
        if (_operations.Count >= BatchConstants.MaxBatchSize)
        {
            throw new InvalidOperationException(
                $"A transactional batch cannot contain more than {BatchConstants.MaxBatchSize} operations.");
        }
    }

    private async ValueTask CreateAsync<TItem>(TItem item, CancellationToken cancellationToken) where TItem : IItem =>
        await Repository<TItem>().CreateAsync(item, cancellationToken).ConfigureAwait(false);

    private async ValueTask UpsertAsync<TItem>(TItem item, CancellationToken cancellationToken) where TItem : IItem =>
        await Repository<TItem>().UpdateAsync(item, cancellationToken: cancellationToken).ConfigureAwait(false);

    // Cosmos transactional batches fail a replace with 404 when the item does
    // not exist; the in-memory upsert would otherwise silently create it.
    private async ValueTask ReplaceAsync<TItem>(TItem item, CancellationToken cancellationToken) where TItem : IItem
    {
        IRepository<TItem> repository = Repository<TItem>();

        if (await repository.ExistsAsync(item.Id, _partitionKey, cancellationToken).ConfigureAwait(false) is false)
        {
            throw new CosmosException(string.Empty, HttpStatusCode.NotFound, 0, string.Empty, 0);
        }

        await repository.UpdateAsync(item, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask DeleteAsync<TItem>(string id, CancellationToken cancellationToken) where TItem : IItem =>
        await Repository<TItem>().DeleteAsync(id, _partitionKey, cancellationToken).ConfigureAwait(false);

    // Resolving through DI targets the same repository instances the
    // InMemoryChangeFeed subscribes to, so batch writes raise change
    // notifications like every other repository operation.
    private IRepository<TItem> Repository<TItem>() where TItem : IItem =>
        serviceProvider.GetRequiredService<IRepository<TItem>>();
}
