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

        public bool IsCriteriaEvaluator { get; } = false;

        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : IItem
        {
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
                    if (orderExpression.OrderType == OrderTypeEnum.OrderBy)
                    {
                        orderedQuery = query.OrderBy(orderExpression.KeySelector);
                    }
                    else if (orderExpression.OrderType == OrderTypeEnum.OrderByDescending)
                    {
                        orderedQuery = query.OrderByDescending(orderExpression.KeySelector);
                    }
                    else if (orderExpression.OrderType == OrderTypeEnum.ThenBy)
                    {
                        orderedQuery = orderedQuery.ThenBy(orderExpression.KeySelector);
                    }
                    else if (orderExpression.OrderType == OrderTypeEnum.ThenByDescending)
                    {
                        orderedQuery = orderedQuery.ThenByDescending(orderExpression.KeySelector);
                    }
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

