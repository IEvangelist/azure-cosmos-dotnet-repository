// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Providers;

namespace Microsoft.Azure.CosmosRepository.Decorators
{
    /// <summary>
    /// A decorator to provide supported LINQ operators for cosmos dbs SQL Core Api.
    /// </summary>
    public class CosmosQueryableDecorator<TItem> where TItem : IItem
    {
        private IQueryable<TItem> _queryable;
        private readonly IRepositoryExpressionProvider _repositoryExpressionProvider;

        internal CosmosQueryableDecorator(IQueryable<TItem> queryable, IRepositoryExpressionProvider repositoryExpressionProvider)
        {
            _queryable = queryable;
            _repositoryExpressionProvider = repositoryExpressionProvider;
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="expression">The expression to filter by.</param>
        /// <returns>The instance of <see cref="CosmosQueryableDecorator{T}"/> for the current query</returns>
        public CosmosQueryableDecorator<TItem> Where(Expression<Func<TItem, bool>> expression)
        {
            _queryable = _queryable.Where(_repositoryExpressionProvider.Build(expression));
            return this;
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="expression">The expression used to order the elements.</param>
        /// <typeparam name="TValue">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <returns>The instance of <see cref="CosmosQueryableDecorator{T}"/> for the current query</returns>
        public CosmosQueryableDecorator<TItem> OrderBy<TValue>(Expression<Func<TItem, TValue>> expression)
        {
            _queryable = _queryable.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <param name="expression">The expression used to order the elements.</param>
        /// <typeparam name="TValue">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <returns>The instance of <see cref="CosmosQueryableDecorator{T}"/> for the current query</returns>
        public CosmosQueryableDecorator<TItem> OrderByDescending<TValue>(Expression<Func<TItem, TValue>> expression)
        {
            _queryable = _queryable.OrderByDescending(expression);
            return this;
        }


    }
}