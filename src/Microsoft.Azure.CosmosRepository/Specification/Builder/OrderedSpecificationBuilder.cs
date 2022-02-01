// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosRepository.Specification.Builder
{
    /// <inheritdoc cref="IOrderedSpecificationBuilder{TItem, TResult}"/>
    internal class OrderedSpecificationBuilder<TItem, TResult> : SpecificationBuilder<TItem, TResult>, IOrderedSpecificationBuilder<TItem, TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        internal OrderedSpecificationBuilder(BaseSpecification<TItem, TResult> specification) : base(specification)
        {
        }

        /// <inheritdoc/>
        public IOrderedSpecificationBuilder<TItem, TResult> ThenBy(
            Expression<Func<TItem, object>> orderExpression)
        {
            Specification.Add(new OrderExpressionInfo<TItem>(orderExpression, OrderTypeEnum.ThenBy));
            return this;
        }

        /// <inheritdoc/>
        public IOrderedSpecificationBuilder<TItem, TResult> ThenByDescending(
            Expression<Func<TItem, object>> orderExpression)
        {
            Specification.Add(new OrderExpressionInfo<TItem>(orderExpression, OrderTypeEnum.ThenByDescending));
            return this;
        }
    }
}