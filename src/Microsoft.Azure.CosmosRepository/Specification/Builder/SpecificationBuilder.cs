// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    class SpecificationBuilder<T,TResult> : ISpecificationBuilder<T,TResult>
        where T : IItem
        where TResult : IQueryResult<T>

    {
        public BaseSpecification<T,TResult> Specification { get; }
        public SpecificationBuilder(BaseSpecification<T, TResult> specification)
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
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="criteria"></param>
        public static ISpecificationBuilder<T, TResult> Where<T, TResult>(
            this ISpecificationBuilder<T, TResult> specificationBuilder,
            Expression<Func<T, bool>> criteria)
            where T : IItem
            where TResult : IQueryResult<T>

        {
            ((List<WhereExpressionInfo<T>>)specificationBuilder.Specification.WhereExpressions).Add(new WhereExpressionInfo<T>(criteria));

            return specificationBuilder;
        }

        /// <summary>
        /// Specify the query result will be ordered by <paramref name="orderExpression"/> in an ascending order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="orderExpression"></param>
        public static IOrderedSpecificationBuilder<T, TResult> OrderBy<T, TResult>(
            this ISpecificationBuilder<T, TResult> specificationBuilder,
            Expression<Func<T, object>> orderExpression)
            where T : IItem
            where TResult : IQueryResult<T>
        {
            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.OrderBy));

            OrderedSpecificationBuilder<T, TResult> orderedSpecificationBuilder = new(specificationBuilder.Specification);

            return orderedSpecificationBuilder;
        }

        /// <summary>
        /// Specify the query result will be ordered by <paramref name="orderExpression"/> in a descending order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="orderExpression"></param>
        public static IOrderedSpecificationBuilder<T, TResult> OrderByDescending<T, TResult>(
            this ISpecificationBuilder<T, TResult> specificationBuilder,
            Expression<Func<T, object>> orderExpression)
            where T : IItem
            where TResult: IQueryResult<T>
        {
            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.OrderByDescending));

            IOrderedSpecificationBuilder<T,TResult> orderedSpecificationBuilder = new OrderedSpecificationBuilder<T, TResult>(specificationBuilder.Specification);

            return orderedSpecificationBuilder;
        }

        /// <summary>
        /// Specify the number of elements to return.
        /// </summary>
        public static ISpecificationBuilder<T, TResult> PageSize<T, TResult>(
            this ISpecificationBuilder<T, TResult> specificationBuilder,
            int pageSize)
            where T : IItem
            where TResult : IPage<T>
        {
            specificationBuilder.Specification.PageSize = pageSize;
            return specificationBuilder;
        }

        /// <summary>
        /// Specify which page of elements to take
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="skip">number of elements to skip</param>
        public static ISpecificationBuilder<T,TResult> PageNumber<T, TResult>(
            this ISpecificationBuilder<T, TResult> specificationBuilder,
            int skip)
            where T : IItem
            where TResult: IPageQueryResult<T>
        {
            specificationBuilder.Specification.PageNumber = skip;
            return specificationBuilder;
        }

        /// <summary>
        /// Specificy a continuation token to use
        /// </summary>
        public static ISpecificationBuilder<T, TResult> ContinutationToken<T,TResult>(
            this ISpecificationBuilder<T, TResult> specificationBuilder,
            string continutationToken)
            where T : IItem
            where TResult : IPage<T>
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
