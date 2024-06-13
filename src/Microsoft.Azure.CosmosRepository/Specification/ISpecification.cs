// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <summary>
/// defines a specification
/// </summary>
public interface ISpecification<TItem, out TResult>
    where TItem : IItem
    where TResult : IQueryResult<TItem>
{
    /// <summary>
    /// A collection of filter expressions used for filtering queries.
    /// </summary>
    IReadOnlyList<WhereExpressionInfo<TItem>> WhereExpressions { get; }
    /// <summary>
    /// A collection of expressions used for sorting.
    /// </summary>
    IReadOnlyList<OrderExpressionInfo<TItem>> OrderExpressions { get; }

    /// <summary>
    /// Processing for updating the query result before returning it from the repository. Given the methods input it should generate the specified TResult />
    /// </summary>
    TResult PostProcessingAction(
        IReadOnlyList<TItem> queryResult,
        int totalCount,
        double charge,
        string? continuationToken);

    /// <summary>
    /// Continuation token used for paging in cosmos. Must set <see cref="UseContinuationToken"/> for continuation token to be applicable
    /// </summary>
    string? ContinuationToken { get; }

    /// <summary>
    /// Select which page shoud be selected in the paginated result
    /// </summary>
    int? PageNumber { get; }

    //TODO: Write doc
    public PartitionKey? PartitionKey { get; }

    /// <summary>
    /// Paginate results, selects how many results should be returned
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Use continuation token instead of page number
    /// </summary>
    bool UseContinuationToken { get; }
}
