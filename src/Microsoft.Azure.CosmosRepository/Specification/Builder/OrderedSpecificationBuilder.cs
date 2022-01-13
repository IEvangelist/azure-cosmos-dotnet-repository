// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <inheritdoc cref="IOrderedSpecificationBuilder{T}"/>
    public class OrderedSpecificationBuilder<T> : IOrderedSpecificationBuilder<T>
        where T : IItem
    {
        /// <inheritdoc/>
        public BaseSpecification<T> Specification { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public OrderedSpecificationBuilder(BaseSpecification<T> specification)
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
        /// <param name="orderBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<T> ThenBy<T>(
            this IOrderedSpecificationBuilder<T> orderBuilder,
            Expression<Func<T, object>> orderExpression)
            where T: IItem
        {
            ((List<OrderExpressionInfo<T>>)orderBuilder.Specification.OrderExpressions)
                .Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.ThenBy));
            return orderBuilder;
        }
        /// <summary>
        /// Adds a ThenByDescending expression to the existing order expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<T> ThenByDescending<T>(
            this IOrderedSpecificationBuilder<T> orderBuilder,
            Expression<Func<T, object>> orderExpression)
            where T : IItem
        {
            ((List<OrderExpressionInfo<T>>)orderBuilder.Specification.OrderExpressions)
                .Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.ThenByDescending));
            return orderBuilder;
        }
    }
}
