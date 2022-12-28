// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// Represent a full set of data from a cosmos query
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IQueryResult<out TItem> where TItem : IItem
{
    /// <summary>
    /// The items that are in the current page.
    /// </summary>
    IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// The amount of RU's the given query cost.
    /// </summary>
    double Charge { get; }
}
