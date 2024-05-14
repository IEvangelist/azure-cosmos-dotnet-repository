// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Builder;

/// <summary>
/// Defines a builder that can build a specification
/// </summary>
/// <typeparam name="TItem">The type of <see cref="IItem"/></typeparam>
/// <typeparam name="TResult">The type of <see cref="IQueryResult{TItem}"/> to return.</typeparam>
public interface ISpecificationBuilder<TItem, TResult>
    where TItem : IItem
    where TResult : IQueryResult<TItem>
{
    /// <summary>
    /// The specification for the <see cref="IItem"/>
    /// </summary>
    BaseSpecification<TItem, TResult> Specification { get; }

    /// <summary>
    /// Provide a filter condition on the current query.
    /// </summary>
    /// <param name="expression">The expression used for filtering.</param>
    /// <returns>An instance of a <see cref="ISpecificationBuilder{TItem,TResult}"/></returns>
    ISpecificationBuilder<TItem, TResult> Where(Expression<Func<TItem, bool>> expression);

    /// <summary>
    /// Provide a filter condition to order the current query.
    /// </summary>
    /// <param name="orderExpression">The expression used to order the query.</param>
    /// <returns>An instance of a <see cref="IOrderedSpecificationBuilder{TItem,TResult}"/></returns>
    IOrderedSpecificationBuilder<TItem, TResult> OrderBy(Expression<Func<TItem, object>> orderExpression);

    /// <summary>
    /// Provide a filter condition to order the current query descending.
    /// </summary>
    /// <param name="orderExpression">The expression used to order the query.</param>
    /// <returns>An instance of a <see cref="IOrderedSpecificationBuilder{TItem,TResult}"/></returns>
    IOrderedSpecificationBuilder<TItem, TResult> OrderByDescending(Expression<Func<TItem, object>> orderExpression);

    /// <summary>
    /// Sets the size of the page.
    /// </summary>
    /// <param name="pageSize">The number of items to return in the page.</param>
    /// <returns>An instance of a <see cref="ISpecificationBuilder{TItem,TResult}"/></returns>
    ISpecificationBuilder<TItem, TResult> PageSize(int pageSize);

    /// <summary>
    /// Sets the page number.
    /// </summary>
    /// <param name="pageNumber">The page number to set.</param>
    /// <returns>An instance of a <see cref="ISpecificationBuilder{TItem,TResult}"/></returns>
    ISpecificationBuilder<TItem, TResult> PageNumber(int pageNumber);

    /// <summary>
    /// Sets the continuation token used for paging.
    /// </summary>
    /// <param name="continuationToken">The token used by Cosmos DB to provide efficient, cost effective paging.</param>
    /// <returns>An instance of a <see cref="ISpecificationBuilder{TItem,TResult}"/></returns>
    ISpecificationBuilder<TItem, TResult> ContinuationToken(string continuationToken);

    /// <summary>
    /// Sets the partition key for the query.
    /// </summary>
    /// <param name="partitionKey">The partition key</param>
    /// <returns>An instance of a <see cref="ISpecificationBuilder{TItem,TResult}"/></returns>
    ISpecificationBuilder<TItem, TResult> PartitionKey(PartitionKey partitionKey);
}
