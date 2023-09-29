// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// 
/// </remarks>
/// <param name="items"></param>
/// <param name="charge"></param>
public class QueryResult<T>(IReadOnlyList<T> items, double charge) : IQueryResult<T>
    where T : IItem
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<T> Items { get; } = items;
    /// <summary>
    /// 
    /// </summary>
    public double Charge { get; } = charge;
}
