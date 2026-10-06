// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal interface IInMemoryBatchRepository
{
    void BeginBatch();

    IReadOnlyList<Action> CommitBatch();

    void RollbackBatch();
}

/// <inheritdoc/>
internal partial class InMemoryRepository<TItem> : IRepository<TItem>, IInMemoryBatchRepository
    where TItem : IItem
{
    private readonly ISpecificationEvaluator _specificationEvaluator;
    private List<Action>? _batchNotifications;
    internal long CurrentTs => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    internal Action<ChangeFeedItemArgs<TItem>>? Changes { get; set; }

    public InMemoryRepository() =>
        _specificationEvaluator = new SpecificationEvaluator();

    public InMemoryRepository(ISpecificationEvaluator specificationEvaluator) =>
        _specificationEvaluator = specificationEvaluator;

    internal (string Key, TItem Item)? FindStoredItem(string id, string partitionKey)
    {
        foreach (KeyValuePair<string, string> entry in InMemoryStorage.GetDictionary<TItem>())
        {
            TItem item = DeserializeItem(entry.Value);

            if (item.Id == id && item.PartitionKey == partitionKey)
            {
                return (entry.Key, item);
            }
        }

        return null;
    }

    private void PublishChanges(ChangeFeedItemArgs<TItem> changes)
    {
        if (_batchNotifications is not null)
        {
            _batchNotifications.Add(() => Changes?.Invoke(changes));
            return;
        }

        Changes?.Invoke(changes);
    }

    void IInMemoryBatchRepository.BeginBatch() => _batchNotifications = [];

    // Draining without publishing lets the caller clear every repository in the
    // batch before any change feed processor runs, so a processor that throws
    // cannot leave a later repository buffering notifications forever.
    IReadOnlyList<Action> IInMemoryBatchRepository.CommitBatch()
    {
        List<Action>? notifications = _batchNotifications;
        _batchNotifications = null;

        return notifications ?? [];
    }

    void IInMemoryBatchRepository.RollbackBatch() => _batchNotifications = null;

    private void NotFound() => throw new CosmosException(string.Empty, HttpStatusCode.NotFound, 0, string.Empty, 0);
    private void Conflict() => throw new CosmosException(string.Empty, HttpStatusCode.Conflict, 0, string.Empty, 0);
}
