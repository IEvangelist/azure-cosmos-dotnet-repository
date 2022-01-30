// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Azure.CosmosRepository.Specification.Builder;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <inheritdoc cref="ISpecification{T,TResult}"/>
    public abstract class BaseSpecification<TItem, TResult> : ISpecification<TItem, TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        /// <summary>
        /// The specification query builder. Always use this object when interacting with the specifications. All other properties are readonly or internal set;
        /// </summary>
        protected ISpecificationBuilder<TItem, TResult> Query { get; }

        /// <summary>
        /// Initialize specification and add filters later
        /// </summary>
        protected BaseSpecification() =>
            Query = new SpecificationBuilder<TItem, TResult>(this);

        /// <summary>
        /// Initialize specification with a list of filter criteria
        /// </summary>
        /// <param name="filters"></param>
        protected BaseSpecification(IEnumerable<Expression<Func<TItem, bool>>> filters)
        {
            Query = new SpecificationBuilder<TItem, TResult>(this);
            foreach (Expression<Func<TItem, bool>> filter in filters)
            {
                Query.Where(filter);
            }
        }

        /// <inheritdoc/>
        public List<WhereExpressionInfo<TItem>> WhereExpressions { get; } =
            new();

        /// <inheritdoc/>
        public List<OrderExpressionInfo<TItem>> OrderExpressions { get; } =
            new();

        /// <inheritdoc/>
        public string ContinuationToken { get; internal set; }

        /// <inheritdoc/>
        public int? PageNumber { get; internal set; }

        /// <inheritdoc/>
        public int PageSize { get; internal set; } = 25;

        /// <inheritdoc/>
        public bool UseContinuationToken { get; internal set; }

        /// <inheritdoc/>
        public abstract TResult PostProcessingAction(
            IReadOnlyList<TItem> queryResult,
            int totalCount,
            double charge,
            string continuationToken);
    }
}