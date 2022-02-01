// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq.Expressions;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosRepository.Specification.Builder
{
    /// <summary>
    /// A set of extension methods that can be used with a given <see cref="ISpecificationBuilder{TItem,TResult}"/>
    /// </summary>
    public static class SpecificationBuilderExtensions
    {
        /// <summary>
        /// Add a search filter added to the query. Multiple filters will be evaluated together
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="criteria"></param>
        public static ISpecificationBuilder<TItem, TResult> Where<TItem, TResult>(
            this ISpecificationBuilder<TItem, TResult> specificationBuilder,
            Expression<Func<TItem, bool>> criteria)
            where TItem : IItem
            where TResult : IQueryResult<TItem>

        {
            specificationBuilder.Specification.Add(new WhereExpressionInfo<TItem>(criteria));
            return specificationBuilder;
        }

        /// <summary>
        /// Specify the query result will be ordered by <paramref name="orderExpression"/> in an ascending order
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="orderExpression"></param>
        public static IOrderedSpecificationBuilder<TItem, TResult> OrderBy<TItem, TResult>(
            this ISpecificationBuilder<TItem, TResult> specificationBuilder,
            Expression<Func<TItem, object>> orderExpression)
            where TItem : IItem
            where TResult : IQueryResult<TItem>
        {
            OrderExpressionInfo<TItem> orderExpressionInfo = new(orderExpression, OrderTypeEnum.OrderBy);
            specificationBuilder.Specification.Add(orderExpressionInfo);

            OrderedSpecificationBuilder<TItem, TResult> orderedSpecificationBuilder = new(specificationBuilder.Specification);

            return orderedSpecificationBuilder;
        }

        /// <summary>
        /// Specify the query result will be ordered by <paramref name="orderExpression"/> in a descending order
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="orderExpression"></param>
        public static IOrderedSpecificationBuilder<TItem, TResult> OrderByDescending<TItem, TResult>(
            this ISpecificationBuilder<TItem, TResult> specificationBuilder,
            Expression<Func<TItem, object>> orderExpression)
            where TItem : IItem
            where TResult: IQueryResult<TItem>
        {
            specificationBuilder.Specification.Add(new OrderExpressionInfo<TItem>(orderExpression, OrderTypeEnum.OrderByDescending));

            IOrderedSpecificationBuilder<TItem,TResult> orderedSpecificationBuilder = new OrderedSpecificationBuilder<TItem, TResult>(specificationBuilder.Specification);

            return orderedSpecificationBuilder;
        }

        /// <summary>
        /// Specify the number of elements to return.
        /// </summary>
        public static ISpecificationBuilder<TItem, TResult> PageSize<TItem, TResult>(
            this ISpecificationBuilder<TItem, TResult> specificationBuilder,
            int pageSize)
            where TItem : IItem
            where TResult : IPage<TItem>
        {
            specificationBuilder.Specification.PageSize = pageSize;
            return specificationBuilder;
        }

        /// <summary>
        /// Specify which page of elements to take
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="skip">number of elements to skip</param>
        public static ISpecificationBuilder<TItem,TResult> PageNumber<TItem, TResult>(
            this ISpecificationBuilder<TItem, TResult> specificationBuilder,
            int skip)
            where TItem : IItem
            where TResult: IPageQueryResult<TItem>
        {
            specificationBuilder.Specification.PageNumber = skip;
            return specificationBuilder;
        }

        /// <summary>
        /// Specify a continuation token to use
        /// </summary>
        public static ISpecificationBuilder<TItem, TResult> ContinuationToken<TItem,TResult>(
            this ISpecificationBuilder<TItem, TResult> specificationBuilder,
            string continuationToken)
            where TItem : IItem
            where TResult : IPage<TItem>
        {
            if(specificationBuilder.Specification.UseContinuationToken == false)
            {
                throw new ArgumentException("Cannot add continuation token to a non continuation token specification", nameof(continuationToken));
            }
            specificationBuilder.Specification.ContinuationToken = continuationToken;
            return specificationBuilder;
        }
    }
}