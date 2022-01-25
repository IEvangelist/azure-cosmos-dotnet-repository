// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <inheritdoc cref="ISpecification{T,TResult}"/>
    public abstract class BaseSpecification<T,TResult> : ISpecification<T, TResult>
        where T : IItem
        where TResult : IQueryResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        protected virtual ISpecificationBuilder<T, TResult> Query { get; }
        /// <summary>
        /// Initialize specificaiton and add filters later
        /// </summary>
public BaseSpecification() =>
             Query = new SpecificationBuilder<T, TResult>(this);

        /// <summary>
        /// Initialize specificaiton with a list of filter criteria
        /// </summary>
        /// <param name="filters"></param>
        public BaseSpecification(Expression<Func<T, bool>>[] filters)
        {
            Query = new SpecificationBuilder<T, TResult>(this);
            foreach(Expression<Func<T, bool>> filter in filters)
            {
                Query.Where(filter);
            }
        }
        /// <inheritdoc/>
        public IReadOnlyList<WhereExpressionInfo<T>> WhereExpressions { get; } = new List<WhereExpressionInfo<T>>();
        /// <inheritdoc/>
        public IReadOnlyList<OrderExpressionInfo<T>> OrderExpressions { get; } = new List<OrderExpressionInfo<T>>();
        /// <inheritdoc/>
        public string ContinutationToken { get; internal set; } = null;
        /// <inheritdoc/>
        public int? PageNumber { get; internal set; } = null;
        /// <inheritdoc/>
        public int PageSize { get; internal set; } = 25;
        /// <inheritdoc/>
        public bool UseContinutationToken { get; internal set; } = false;

        /// <inheritdoc/>
        public abstract TResult PostProcessingAction(IReadOnlyList<T> queryResult, int totalCount, double charge, string continuationToken);

    }
}
