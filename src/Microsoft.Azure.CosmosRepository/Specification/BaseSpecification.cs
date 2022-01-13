// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <inheritdoc cref="ISpecification{T}"/>
    public abstract class BaseSpecification<T> : ISpecification<T>
        where T : IItem
    {
        /// <summary>
        /// 
        /// </summary>
        protected virtual ISpecificationBuilder<T> Query { get; }
        /// <summary>
        /// Initialize specificaiton and add filters later
        /// </summary>
        public BaseSpecification()
        {
            Query = new SpecificationBuilder<T>(this);

        }
        /// <summary>
        /// Initialize specificaiton with a list of filter criteria
        /// </summary>
        /// <param name="filters"></param>
        public BaseSpecification(Expression<Func<T, bool>>[] filters)
        {
            Query = new SpecificationBuilder<T>(this);
            foreach(Expression<Func<T, bool>> filter in filters)
            {
                Query.Where(filter);
            }
        }
        /// <inheritdoc/>
        public IEnumerable<WhereExpressionInfo<T>> WhereExpressions { get; } = new List<WhereExpressionInfo<T>>();
        /// <inheritdoc/>
        public IEnumerable<OrderExpressionInfo<T>> OrderExpressions { get; } = new List<OrderExpressionInfo<T>>();
        /// <inheritdoc/>
        public string ContinutationToken { get; internal set; } = null;
        /// <inheritdoc/>
        public int? PageNumber { get; internal set; } = null;
        /// <inheritdoc/>
        public int PageSize { get; internal set; } = 25;
        /// <inheritdoc/>
        public bool UseContinutationToken { get; internal set; } = true;
    }
}
