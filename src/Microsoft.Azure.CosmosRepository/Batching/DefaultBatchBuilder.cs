namespace Microsoft.Azure.CosmosRepository;

internal sealed class DefaultBatchBuilder(
    string partitionKey,
    ICosmosContainerService containerService) : IBatchBuilder
{
    private readonly List<Action<TransactionalBatch>> _operations = [];
    private readonly HashSet<Type> _seenTypes = [];

    public IBatchBuilder CreateItem<TItem>(TItem item) where TItem : IItem =>
        Add(item, batch => batch.CreateItem(item));

    public IBatchBuilder ReplaceItem<TItem>(TItem item) where TItem : IItem
    {
        ArgumentNullException.ThrowIfNull(item);
        string id = item.Id;
        string? etag = GetEtag(item);
        return Add(item, batch => batch.ReplaceItem(id, item, BatchRequestOptions.Create(etag)));
    }

    public IBatchBuilder UpsertItem<TItem>(TItem item) where TItem : IItem
    {
        ArgumentNullException.ThrowIfNull(item);
        string? etag = GetEtag(item);
        return Add(item, batch => batch.UpsertItem(item, BatchRequestOptions.Create(etag)));
    }

    public IBatchBuilder DeleteItem<TItem>(TItem item) where TItem : IItem
    {
        ArgumentNullException.ThrowIfNull(item);
        string id = item.Id;
        return Add(item, batch => batch.DeleteItem(id));
    }

    public IBatchBuilder DeleteItem<TItem>(string id) where TItem : IItem
    {
        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        EnsureCapacity();

        _seenTypes.Add(typeof(TItem));
        _operations.Add(batch => batch.DeleteItem(id));

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

        // This call ensures that all the types are valid for the same container
        Container container = await containerService.GetContainerAsync(_seenTypes.ToList())
            .ConfigureAwait(false);

        TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(partitionKey));

        foreach (Action<TransactionalBatch> operation in _operations)
        {
            operation(batch);
        }

        using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new BatchOperationException(response);
        }
    }

    private IBatchBuilder Add<TItem>(TItem item, Action<TransactionalBatch> operation) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        EnsureCapacity();

        _seenTypes.Add(typeof(TItem));
        _operations.Add(operation);

        return this;
    }

    private static string? GetEtag(IItem item) =>
        item is IItemWithEtag itemWithEtag ? itemWithEtag.Etag : null;

    private void ValidatePartitionKey(IItem item)
    {
        if (!string.Equals(item.PartitionKey, partitionKey, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"The item partition key '{item.PartitionKey}' does not match the batch partition key '{partitionKey}'.",
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
}
