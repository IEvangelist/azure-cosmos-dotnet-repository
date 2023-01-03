// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <inheritdoc/>
public class ContinuationTokenSpecification<TItem> : BaseSpecification<TItem, IPage<TItem>>
    where TItem : IItem
{
    /// <summary>
    /// Default constructor to set all parameters yourself
    /// </summary>
    protected ContinuationTokenSpecification() =>
        UseContinuationToken = true;

    /// <summary>
    /// Constructor for specifying the token and page size
    /// </summary>
    /// <param name="continuationToken"></param>
    /// <param name="pageSize"></param>
    public ContinuationTokenSpecification(string continuationToken, int pageSize)
    {
        UseContinuationToken = true;
        Query.ContinuationToken(continuationToken);
        Query.PageSize(pageSize);
    }

    /// <summary>
    /// When scrolling through multiple pages reuse the same specification and use this method to update the continuation token
    /// </summary>
    /// <param name="continuationToken"></param>
    public void UpdateContinuationToken(string continuationToken) =>
        Query.ContinuationToken(continuationToken);

    /// <inheritdoc/>
    public override IPage<TItem> PostProcessingAction(
        IReadOnlyList<TItem> queryResult,
        int totalCount,
        double charge,
        string? continuationToken) =>
        new Page<TItem>(totalCount, PageSize, queryResult, charge, continuationToken);
}