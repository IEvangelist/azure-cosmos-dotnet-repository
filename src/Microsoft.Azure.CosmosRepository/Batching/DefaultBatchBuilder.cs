namespace Microsoft.Azure.CosmosRepository;

internal sealed class DefaultBatchBuilder(
    string partitionKey,
    Type seedType,
    ICosmosContainerService containerService) : IBatchBuilder
{
    private readonly List<PendingOp> _operations = [];
    private readonly HashSet<Type> _seenTypes = [seedType];

    public IBatchBuilder CreateItem<TItem>(TItem item) where TItem : IItem =>
        Add(OpKind.Create, item);

    public IBatchBuilder ReplaceItem<TItem>(TItem item) where TItem : IItem =>
        Add(OpKind.Replace, item);

    public IBatchBuilder UpsertItem<TItem>(TItem item) where TItem : IItem =>
        Add(OpKind.Upsert, item);

    public IBatchBuilder DeleteItem<TItem>(TItem item) where TItem : IItem =>
        Add(OpKind.Delete, item);

    public IBatchBuilder DeleteItem<TItem>(string id) where TItem : IItem
    {
        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        EnsureCapacity();

        _seenTypes.Add(typeof(TItem));
        _operations.Add(new(OpKind.Delete, typeof(TItem), null, id, null));

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

        foreach (PendingOp operation in _operations)
        {
            switch (operation.Kind)
            {
                case OpKind.Create:
                    batch.CreateItem(operation.Item);
                    break;
                case OpKind.Replace:
                    batch.ReplaceItem(operation.Id, operation.Item, CreateRequestOptions(operation.Etag));
                    break;
                case OpKind.Upsert:
                    batch.UpsertItem(operation.Item, CreateRequestOptions(operation.Etag));
                    break;
                case OpKind.Delete:
                    batch.DeleteItem(operation.Id);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown batch operation kind: {operation.Kind}");
            }
        }

        TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new BatchOperationException(response);
        }

        response.Dispose();
    }

    private IBatchBuilder Add<TItem>(OpKind kind, TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        EnsureCapacity();

        _seenTypes.Add(typeof(TItem));

        _operations.Add(new(
            kind,
            typeof(TItem),
            item,
            item.Id,
            kind is OpKind.Replace or OpKind.Upsert && item is IItemWithEtag itemWithEtag
                ? itemWithEtag.Etag
                : null));

        return this;
    }


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

    private static TransactionalBatchItemRequestOptions CreateRequestOptions(string? etag)
    {
        TransactionalBatchItemRequestOptions options = new();

        if (!string.IsNullOrWhiteSpace(etag))
        {
            options.IfMatchEtag = etag;
        }

        return options;
    }

    private enum OpKind
    {
        Create,
        Replace,
        Upsert,
        Delete
    }

    private sealed class PendingOp
    {
        public PendingOp(
            OpKind kind,
            Type itemType,
            object? item,
            string id,
            string? etag)
        {
            Kind = kind;
            ItemType = itemType;
            Item = item;
            Id = id;
            Etag = etag;
        }

        public OpKind Kind { get; }

        public Type ItemType { get; }

        public object? Item { get; }

        public string Id { get; }

        public string? Etag { get; }
    }
}
