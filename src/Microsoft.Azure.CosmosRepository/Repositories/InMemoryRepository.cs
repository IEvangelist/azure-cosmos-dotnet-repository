// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory;
using Microsoft.Azure.CosmosRepository.Specification.Evaluator;

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

/// <inheritdoc/>
internal partial class InMemoryRepository<TItem> : IRepository<TItem>
    where TItem : IItem
{
    private readonly ISpecificationEvaluator _specificationEvaluator;
    internal long CurrentTs => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    internal ConcurrentDictionary<string, string> Items { get; } = new();
    internal Action<ChangeFeedItemArgs<TItem>>? Changes { get; set; }

    public InMemoryRepository() =>
        _specificationEvaluator = new SpecificationEvaluator();

    public InMemoryRepository(ISpecificationEvaluator specificationEvaluator) =>
        _specificationEvaluator = specificationEvaluator;

    private void NotFound() => throw new CosmosException(string.Empty, HttpStatusCode.NotFound, 0, string.Empty, 0);
    private void Conflict() => throw new CosmosException(string.Empty, HttpStatusCode.Conflict, 0, string.Empty, 0);
}