// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <inheritdoc cref="IOrderedSpecificationBuilder{T, TResult}"/>
    public class OrderedSpecificationBuilder<T, TResult> : IOrderedSpecificationBuilder<T, TResult>
        where T : IItem
        where TResult : IQueryResult<T>
    {
        /// <inheritdoc/>
        public BaseSpecification<T,TResult> Specification { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public OrderedSpecificationBuilder(BaseSpecification<T, TResult> specification)
        {
            Specification = specification;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class OrderedBuilderExtensions
    {

        /// <summary>
        /// Adds a ThenBy expression to the existing order expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="orderBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<T, TResult> ThenBy<T, TResult>(
            this IOrderedSpecificationBuilder<T, TResult> orderBuilder,
            Expression<Func<T, object>> orderExpression)
            where T: IItem
            where TResult: IQueryResult<T>
        {
            ((List<OrderExpressionInfo<T>>)orderBuilder.Specification.OrderExpressions)
                .Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.ThenBy));
            return orderBuilder;
        }
        /// <summary>
        /// Adds a ThenByDescending expression to the existing order expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="orderBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<T, TResult> ThenByDescending<T, TResult>(
            this IOrderedSpecificationBuilder<T, TResult> orderBuilder,
            Expression<Func<T, object>> orderExpression)
            where T : IItem
            where TResult : IQueryResult<T>
        {
            ((List<OrderExpressionInfo<T>>)orderBuilder.Specification.OrderExpressions)
                .Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.ThenByDescending));
            return orderBuilder;
        }
    }
}
