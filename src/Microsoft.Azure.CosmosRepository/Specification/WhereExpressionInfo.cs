// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <summary>
/// Container class for a where predicate
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class WhereExpressionInfo<TItem>
{
    /// <summary>
    /// Constructor for creating a where expression
    /// </summary>
    /// <param name="filter"></param>
    public WhereExpressionInfo(Expression<Func<TItem, bool>> filter) =>
        Filter = filter;

    /// <summary>
    /// A predicate that is used for filtering. Given an item of <typeparamref name="TItem"/> a function evalut
    /// </summary>
    public Expression<Func<TItem, bool>> Filter { get; }
}