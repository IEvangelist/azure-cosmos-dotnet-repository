// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator
{
    class OrderEvaluator : IEvaluator
    {
        private OrderEvaluator() { }
        public static OrderEvaluator Instance { get; } = new OrderEvaluator();

        public bool IsFilterEvaluator { get; } = false;

        public IQueryable<T> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification)
            where T : IItem
            where TResult : IQueryResult<T>
        {
            if(specification.ContinutationToken != null && specification.ContinutationToken != "")
            {
                //Ordering is handled with the token
                return query;
            }

            if (specification.OrderExpressions != null)
            {
                if (specification.OrderExpressions.Count(x => x.OrderType == OrderTypeEnum.OrderBy
                        || x.OrderType == OrderTypeEnum.OrderByDescending) > 1)
                {
                    throw new ArgumentException("Multiple OrderBy expressions found only use one and then chain with ThenBy and TheByDescending");
                }

                IOrderedQueryable<T> orderedQuery = null;
                foreach (OrderExpressionInfo<T> orderExpression in specification.OrderExpressions)
                {
                    orderedQuery = orderExpression.OrderType switch
                    {
                        OrderTypeEnum.OrderBy => query.OrderBy(orderExpression.KeySelector),
                        OrderTypeEnum.OrderByDescending => query.OrderByDescending(orderExpression.KeySelector),
                        OrderTypeEnum.ThenBy => orderedQuery.ThenBy(orderExpression.KeySelector),
                        OrderTypeEnum.ThenByDescending => orderedQuery.ThenByDescending(orderExpression.KeySelector),
                        _ => throw new NotImplementedException("Unknown value of OrderTypeEnum")
                    };
                }

                if (orderedQuery != null)
                {
                    query = orderedQuery;
                }
            }

            return query;
        }
    }
}

