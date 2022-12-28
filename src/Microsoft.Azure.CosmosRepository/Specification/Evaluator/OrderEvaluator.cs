// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator;

internal class OrderEvaluator : IEvaluator
{
    public bool IsFilterEvaluator => false;

    public IQueryable<TItem> GetQuery<TItem, TResult>(
        IQueryable<TItem> query,
        ISpecification<TItem, TResult> specification)
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        if (specification.OrderExpressions.Any() is false)
        {
            return query;
        }

        if (specification.OrderExpressions.Count(x =>
                x.OrderType is OrderTypeEnum.OrderBy or OrderTypeEnum.OrderByDescending) > 1)
        {
            throw new ArgumentException(
                "Multiple OrderBy expressions found only use one and then chain with ThenBy and TheByDescending");
        }

        IOrderedQueryable<TItem>? orderedQuery = null;

        foreach (OrderExpressionInfo<TItem> orderExpression in specification.OrderExpressions)
        {
            orderedQuery = orderExpression.OrderType switch
            {
                OrderTypeEnum.OrderBy => query.OrderBy(orderExpression.KeySelector),

                OrderTypeEnum.OrderByDescending => query.OrderByDescending(orderExpression.KeySelector),

                OrderTypeEnum.ThenBy => orderedQuery?.ThenBy(orderExpression.KeySelector) ??
                                        throw new InvalidOperationException(
                                            "You cannot use ThenBy before using either OrderBy or OrderByDescending"),

                OrderTypeEnum.ThenByDescending => orderedQuery?.ThenByDescending(orderExpression.KeySelector) ??
                                                  throw new InvalidOperationException(
                                                      "You cannot use ThenByDescending before using either OrderBy or OrderByDescending"),

                _ => throw new ArgumentOutOfRangeException($"{orderExpression.OrderType} is not supported")
            };
        }

        if (orderedQuery != null)
        {
            query = orderedQuery;
        }

        return query;
    }
}