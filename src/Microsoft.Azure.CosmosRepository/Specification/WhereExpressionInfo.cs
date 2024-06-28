// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <summary>
/// Container class for a where predicate
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <remarks>
/// Constructor for creating a where expression
/// </remarks>
/// <param name="filter"></param>
public class WhereExpressionInfo<TItem>(Expression<Func<TItem, bool>> filter)
{
    /// <summary>
    /// A predicate that is used for filtering. Given an item of <typeparamref name="TItem"/> a function evalut
    /// </summary>
    public Expression<Func<TItem, bool>> Filter { get; } = filter;
}