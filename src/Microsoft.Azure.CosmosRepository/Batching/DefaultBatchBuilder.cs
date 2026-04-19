namespace Microsoft.Azure.CosmosRepository;

internal sealed class DefaultBatchBuilder(
    string partitionKey,
    Type seedType,
    ICosmosContainerService containerService,
    ICosmosItemConfigurationProvider configProvider) : IBatchBuilder
{
    private readonly string _partitionKey = partitionKey;
    private readonly string _seedContainerName = configProvider.GetItemConfiguration(seedType).ContainerName;
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

        ValidateContainer(typeof(TItem));
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

        Container container = await containerService.GetContainerAsync(_seenTypes.ToList())
            .ConfigureAwait(false);

        TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(_partitionKey));

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

        ValidateContainer(typeof(TItem));
        ValidatePartitionKey(item);

        _operations.Add(new(
            kind,
            typeof(TItem),
            item,
            item.Id,
            (kind is OpKind.Replace or OpKind.Upsert) && item is IItemWithEtag itemWithEtag
                ? itemWithEtag.Etag
                : null));

        return this;
    }

    private void ValidateContainer(Type itemType)
    {
        if (!_seenTypes.Add(itemType))
        {
            return;
        }

        string containerName = configProvider.GetItemConfiguration(itemType).ContainerName;

        if (!string.Equals(containerName, _seedContainerName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"The item type {itemType.Name} is not configured to use the same container as {seedType.Name}.");
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
