// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace

namespace Microsoft.Azure.CosmosRepository;

/// <inheritdoc/>
internal partial class InMemoryRepository<TItem> : IRepository<TItem>
    where TItem : IItem
{
    private readonly ISpecificationEvaluator _specificationEvaluator;
    internal long CurrentTs => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    internal Action<ChangeFeedItemArgs<TItem>>? Changes { get; set; }

    public InMemoryRepository() =>
        _specificationEvaluator = new SpecificationEvaluator();

    public InMemoryRepository(ISpecificationEvaluator specificationEvaluator) =>
        _specificationEvaluator = specificationEvaluator;

    private void NotFound() => throw new CosmosException(string.Empty, HttpStatusCode.NotFound, 0, string.Empty, 0);
    private void Conflict() => throw new CosmosException(string.Empty, HttpStatusCode.Conflict, 0, string.Empty, 0);

    public ValueTask<TItem?> TryGetAsync(string id, IEnumerable<string> partitionKeyValues, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TItem?> TryGetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TItem> GetAsync(string id, IEnumerable<string> partitionKeyValues, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IEnumerable<TItem>> GetAsync(PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IEnumerable<TItem>> GetAsync(PartitionKey partitionKey, Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> ExistsAsync(string id, IEnumerable<string> partitionKeyValues, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate, PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<int> CountAsync(Expression<Func<TItem, bool>> predicate, IEnumerable<string> partitionKeyValues, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<int> CountAsync(Expression<Func<TItem, bool>> predicate, PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<int> CountAsync(PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IPageQueryResult<TItem>> PageAsync(PartitionKey partitionKey, Expression<Func<TItem, bool>>? predicate = null, int pageNumber = 1, int pageSize = 25, bool returnTotal = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IPage<TItem>> PageAsync(PartitionKey partitionKey, Expression<Func<TItem, bool>>? predicate = null, int pageSize = 25, string? continuationToken = null, bool returnTotal = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, string? etag = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, IEnumerable<string> partitionKeyValues, string? etag = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, PartitionKey partitionKey, string? etag = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeleteAsync(string id, IEnumerable<string> partitionKeyValues, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}