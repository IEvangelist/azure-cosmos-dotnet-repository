// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <summary>
/// A specification used for the Offset and Limit pattern
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class OffsetByPageNumberSpecification<TItem> : BaseSpecification<TItem, IPageQueryResult<TItem>>
    where TItem : IItem
{
    /// <summary>
    /// Helper ctor to set page number and page size
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    public OffsetByPageNumberSpecification(int pageNumber = 1, int pageSize = 25)
    {
        Query.PageSize(pageSize);
        Query.PageNumber(pageNumber);
    }

    /// <summary>
    /// Update the specification to get the next page of the result
    /// </summary>
    public void NextPage() =>
        Query.PageNumber(PageNumber + 1 ?? 1);

    /// <summary>
    /// Update the specification to get the previous page of the result
    /// </summary>
    public void PreviousPage() =>
        Query.PageNumber(PageNumber - 1 ?? 1);

    /// <inheritdoc/>
    public override IPageQueryResult<TItem> PostProcessingAction(
        IReadOnlyList<TItem> queryResult,
        int totalCount,
        double charge,
        string? continuationToken) => new PageQueryResult<TItem>(
            totalCount,
            PageNumber,
            PageSize,
            queryResult,
            charge);
}