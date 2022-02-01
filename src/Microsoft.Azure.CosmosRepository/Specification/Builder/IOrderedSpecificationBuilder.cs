// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosRepository.Specification.Builder
{
    /// <inheritdoc cref="ISpecificationBuilder{T, TResult}"/>
    public interface IOrderedSpecificationBuilder<TItem, TResult> : ISpecificationBuilder<TItem, TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        /// <summary>
        /// Adds a second a second expression used to order the query after the initial query.
        /// </summary>
        /// <param name="orderExpression">The expression used to order the <see cref="IItem"/></param>
        /// <remarks>A composite index is required in Cosmos DB to use this feature.</remarks>
        /// <returns>An instance of a <see cref="IOrderedSpecificationBuilder{TItem,TResult}"/></returns>
        IOrderedSpecificationBuilder<TItem, TResult> ThenBy(
            Expression<Func<TItem, object>> orderExpression);

        /// <summary>
        /// Adds a second a second expression used to order the query after the initial query.
        /// </summary>
        /// <param name="orderExpression">The expression used to order the <see cref="IItem"/></param>
        /// <remarks>A composite index is required in Cosmos DB to use this feature.</remarks>
        /// <returns>An instance of a <see cref="IOrderedSpecificationBuilder{TItem,TResult}"/></returns>
        public IOrderedSpecificationBuilder<TItem, TResult> ThenByDescending(
            Expression<Func<TItem, object>> orderExpression);
    }
}
