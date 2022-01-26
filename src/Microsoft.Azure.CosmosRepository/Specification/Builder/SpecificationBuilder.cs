// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    class SpecificationBuilder<TItem,TResult> : ISpecificationBuilder<TItem,TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>

    {
        public BaseSpecification<TItem,TResult> Specification { get; }
        public SpecificationBuilder(BaseSpecification<TItem, TResult> specification)
        {
            Specification = specification;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class SpecificationBuilderExtensions
    {
        /// <summary>
        /// Add a search filter added to the query. Multiple filters will be evaluated togheter
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
            ((List<WhereExpressionInfo<TItem>>)specificationBuilder.Specification.WhereExpressions).Add(new WhereExpressionInfo<TItem>(criteria));

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
            ((List<OrderExpressionInfo<TItem>>)specificationBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<TItem>(orderExpression, OrderTypeEnum.OrderBy));

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
            ((List<OrderExpressionInfo<TItem>>)specificationBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<TItem>(orderExpression, OrderTypeEnum.OrderByDescending));

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
        /// Specificy a continuation token to use
        /// </summary>
        public static ISpecificationBuilder<TItem, TResult> ContinutationToken<TItem,TResult>(
            this ISpecificationBuilder<TItem, TResult> specificationBuilder,
            string continutationToken)
            where TItem : IItem
            where TResult : IPage<TItem>
        {
            if(specificationBuilder.Specification.UseContinutationToken == false)
            {
                throw new ArgumentException("Cannot add continuationtoken to a non continutation token specification", "continutationToken");
            }
            specificationBuilder.Specification.ContinutationToken = continutationToken;
            return specificationBuilder;
        }
    }
}
