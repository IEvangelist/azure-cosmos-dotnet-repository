// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <summary>
/// A specification used for getting all results in a <see cref="QueryResult{T}"/>
/// </summary>
/// <typeparam name="TItem">The type of <see cref="IItem"/> being queried.</typeparam>
public class DefaultSpecification<TItem> : BaseSpecification<TItem, IQueryResult<TItem>>
    where TItem : IItem
{
    /// <inheritdoc/>
    public override IQueryResult<TItem> PostProcessingAction(
        IReadOnlyList<TItem> queryResult,
        int totalCount,
        double charge,
        string? continuationToken) =>
        new QueryResult<TItem>(queryResult, charge);
}