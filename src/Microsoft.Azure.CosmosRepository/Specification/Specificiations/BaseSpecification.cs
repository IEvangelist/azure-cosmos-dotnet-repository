// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        protected virtual ISpecificationBuilder<TItem, TResult> Query { get; }
        /// <summary>
        /// Initialize specificaiton and add filters later
        /// </summary>
        public BaseSpecification() =>
             Query = new SpecificationBuilder<TItem, TResult>(this);

        /// <summary>
        /// Initialize specificaiton with a list of filter criteria
        /// </summary>
        /// <param name="filters"></param>
        public BaseSpecification(Expression<Func<TItem, bool>>[] filters)
        {
            Query = new SpecificationBuilder<TItem, TResult>(this);
            foreach(Expression<Func<TItem, bool>> filter in filters)
            {
                Query.Where(filter);
            }
        }
        /// <inheritdoc/>
        public IReadOnlyList<WhereExpressionInfo<TItem>> WhereExpressions { get; } = new List<WhereExpressionInfo<TItem>>();
        /// <inheritdoc/>
        public IReadOnlyList<OrderExpressionInfo<TItem>> OrderExpressions { get; } = new List<OrderExpressionInfo<TItem>>();
        /// <inheritdoc/>
        public string ContinutationToken { get; internal set; } = null;
        /// <inheritdoc/>
        public int? PageNumber { get; internal set; } = null;
        /// <inheritdoc/>
        public int PageSize { get; internal set; } = 25;
        /// <inheritdoc/>
        public bool UseContinutationToken { get; internal set; } = false;

        /// <inheritdoc/>
        public abstract TResult PostProcessingAction(IReadOnlyList<TItem> queryResult, int totalCount, double charge, string continuationToken);

    }
}
