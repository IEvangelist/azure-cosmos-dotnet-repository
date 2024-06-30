// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <inheritdoc cref="ISpecification{T,TResult}"/>
public abstract class BaseSpecification<TItem, TResult> : ISpecification<TItem, TResult>
    where TItem : IItem
    where TResult : IQueryResult<TItem>
{
    private readonly List<WhereExpressionInfo<TItem>> _whereExpressions = [];
    private readonly List<OrderExpressionInfo<TItem>> _orderExpressions = [];

    /// <summary>
    /// The specification query builder. Always use this object when interacting with the specifications. All other properties are readonly or internal set;
    /// </summary>
    protected ISpecificationBuilder<TItem, TResult> Query { get; }

    /// <summary>
    /// Initialize specification and add filters later
    /// </summary>
    protected BaseSpecification() =>
        Query = new SpecificationBuilder<TItem, TResult>(this);

    internal void Add(WhereExpressionInfo<TItem> expression) =>
        _whereExpressions.Add(expression);

    internal void Add(OrderExpressionInfo<TItem> expression) =>
        _orderExpressions.Add(expression);

    /// <inheritdoc/>
    public IReadOnlyList<WhereExpressionInfo<TItem>> WhereExpressions =>
        _whereExpressions;

    /// <inheritdoc/>
    public IReadOnlyList<OrderExpressionInfo<TItem>> OrderExpressions =>
        _orderExpressions;

    /// <inheritdoc/>
    public string? ContinuationToken { get; internal set; }

    /// <inheritdoc/>
    public int? PageNumber { get; internal set; }

    /// <inheritdoc/>
    public int PageSize { get; internal set; } = 25;

    /// <inheritdoc/>
    public bool UseContinuationToken { get; internal set; }

    /// <inheritdoc/>
    public abstract TResult PostProcessingAction(
        IReadOnlyList<TItem> queryResult,
        int totalCount,
        double charge,
        string? continuationToken);
}