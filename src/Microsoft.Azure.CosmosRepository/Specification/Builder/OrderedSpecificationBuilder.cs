// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <inheritdoc cref="IOrderedSpecificationBuilder{TItem, TResult}"/>
    public class OrderedSpecificationBuilder<TItem, TResult> : IOrderedSpecificationBuilder<TItem, TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        /// <inheritdoc/>
        public BaseSpecification<TItem, TResult> Specification { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public OrderedSpecificationBuilder(BaseSpecification<TItem, TResult> specification) => 
            Specification = specification;
        
    }
    /// <summary>
    /// 
    /// </summary>
    public static class OrderedBuilderExtensions
    {

        /// <summary>
        /// Adds a ThenBy expression to the existing order expression
        /// All ThenBy expressions requires a composite index that matched the ordering clause
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="orderBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<TItem, TResult> ThenBy<TItem, TResult>(
            this IOrderedSpecificationBuilder<TItem, TResult> orderBuilder,
            Expression<Func<TItem, object>> orderExpression)
            where TItem : IItem
            where TResult: IQueryResult<TItem>
        {
            ((List<OrderExpressionInfo<TItem>>)orderBuilder.Specification.OrderExpressions)
                .Add(new OrderExpressionInfo<TItem>(orderExpression, OrderTypeEnum.ThenBy));
            return orderBuilder;
        }
        /// <summary>
        /// Adds a ThenByDescending expression to the existing order expression
        /// All ThenByDescending expressions requires a composite index that matched the ordering clause
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="orderBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<TItem, TResult> ThenByDescending<TItem, TResult>(
            this IOrderedSpecificationBuilder<TItem, TResult> orderBuilder,
            Expression<Func<TItem, object>> orderExpression)
            where TItem : IItem
            where TResult : IQueryResult<TItem>
        {
            ((List<OrderExpressionInfo<TItem>>)orderBuilder.Specification.OrderExpressions)
                .Add(new OrderExpressionInfo<TItem>(orderExpression, OrderTypeEnum.ThenByDescending));
            return orderBuilder;
        }
    }
}
