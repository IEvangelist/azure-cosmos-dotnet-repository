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
}