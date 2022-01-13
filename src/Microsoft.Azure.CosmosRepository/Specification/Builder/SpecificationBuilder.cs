// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    class SpecificationBuilder<T> : ISpecificationBuilder<T>
        where T : IItem
    {
        public BaseSpecification<T> Specification { get; }
        public SpecificationBuilder(BaseSpecification<T> specification)
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
        /// <param name="specificationBuilder"></param>
        /// <param name="criteria"></param>
        public static ISpecificationBuilder<T> Where<T>(
            this ISpecificationBuilder<T> specificationBuilder,
            Expression<Func<T, bool>> criteria)
            where T : IItem

        {
            ((List<WhereExpressionInfo<T>>)specificationBuilder.Specification.WhereExpressions).Add(new WhereExpressionInfo<T>(criteria));

            return specificationBuilder;
        }

        /// <summary>
        /// Specify the query result will be ordered by <paramref name="orderExpression"/> in an ascending order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="orderExpression"></param>
        public static IOrderedSpecificationBuilder<T> OrderBy<T>(
            this ISpecificationBuilder<T> specificationBuilder,
            Expression<Func<T, object>> orderExpression)
            where T : IItem
        {
            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.OrderBy));

            OrderedSpecificationBuilder<T> orderedSpecificationBuilder = new(specificationBuilder.Specification);

            return orderedSpecificationBuilder;
        }

        /// <summary>
        /// Specify the query result will be ordered by <paramref name="orderExpression"/> in a descending order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="orderExpression"></param>
        public static IOrderedSpecificationBuilder<T> OrderByDescending<T>(
            this ISpecificationBuilder<T> specificationBuilder,
            Expression<Func<T, object>> orderExpression)
            where T : IItem
        {
            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.OrderByDescending));

            OrderedSpecificationBuilder<T> orderedSpecificationBuilder = new(specificationBuilder.Specification);

            return orderedSpecificationBuilder;
        }

        /// <summary>
        /// Specify the number of elements to return.
        /// </summary>
        public static ISpecificationBuilder<T> PageSize<T>(
            this ISpecificationBuilder<T> specificationBuilder,
            int pageSize)
            where T : IItem
        {
            specificationBuilder.Specification.PageSize = pageSize;
            return specificationBuilder;
        }

        /// <summary>
        /// Specify which page of elements to take
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="skip">number of elements to skip</param>
        public static ISpecificationBuilder<T> PageNumber<T>(
            this ISpecificationBuilder<T> specificationBuilder,
            int skip)
            where T : IItem
        {
            specificationBuilder.Specification.PageNumber = skip;
            return specificationBuilder;
        }

        /// <summary>
        /// Specificy a continuation token to use
        /// </summary>
        public static ISpecificationBuilder<T> ContinutationToken<T>(
            this ISpecificationBuilder<T> specificationBuilder,
            string continutationToken)
            where T : IItem
        {
            specificationBuilder.Specification.ContinutationToken = continutationToken;
            specificationBuilder.Specification.UseContinutationToken = true;
            return specificationBuilder;
        }

        /// <summary>
        /// Specificy a continuation token to use
        /// </summary>
        public static ISpecificationBuilder<T> DisableContinuationtoken<T>(
            this ISpecificationBuilder<T> specificationBuilder)
            where T : IItem
        {
            specificationBuilder.Specification.UseContinutationToken = false;
            return specificationBuilder;
        }

    }
}
