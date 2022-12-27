// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosRepository.Specification.Builder;

internal class SpecificationBuilder<TItem, TResult> : ISpecificationBuilder<TItem, TResult>
    where TItem : IItem
    where TResult : IQueryResult<TItem>
{
    public BaseSpecification<TItem, TResult> Specification { get; }

    public SpecificationBuilder(BaseSpecification<TItem, TResult> specification) =>
        Specification = specification;

    /// <inheritdoc/>
    public ISpecificationBuilder<TItem, TResult> Where(Expression<Func<TItem, bool>> expression)
    {
        Specification.Add(new WhereExpressionInfo<TItem>(expression));
        return this;
    }

    /// <inheritdoc/>
    public IOrderedSpecificationBuilder<TItem, TResult> OrderBy(Expression<Func<TItem, object>> orderExpression)
    {
        OrderExpressionInfo<TItem> orderExpressionInfo = new(orderExpression, OrderTypeEnum.OrderBy);
        Specification.Add(orderExpressionInfo);

        OrderedSpecificationBuilder<TItem, TResult> orderedSpecificationBuilder = new(Specification);

        return orderedSpecificationBuilder;
    }

    /// <inheritdoc/>
    public IOrderedSpecificationBuilder<TItem, TResult> OrderByDescending(Expression<Func<TItem, object>> orderExpression)
    {
        Specification.Add(new OrderExpressionInfo<TItem>(orderExpression, OrderTypeEnum.OrderByDescending));

        IOrderedSpecificationBuilder<TItem, TResult> orderedSpecificationBuilder =
            new OrderedSpecificationBuilder<TItem, TResult>(Specification);

        return orderedSpecificationBuilder;
    }

    /// <inheritdoc/>
    public ISpecificationBuilder<TItem, TResult> PageSize(int pageSize)
    {
        Specification.PageSize = pageSize;
        return this;
    }


    public ISpecificationBuilder<TItem, TResult> PageNumber(int pageNumber)
    {
        Specification.PageNumber = pageNumber;
        return this;
    }

    /// <inheritdoc/>
    public ISpecificationBuilder<TItem, TResult> ContinuationToken(string continuationToken)
    {
        if (Specification.UseContinuationToken == false)
        {
            throw new ArgumentException("Cannot add continuation token to a non continuation token specification",
                nameof(continuationToken));
        }

        Specification.ContinuationToken = continuationToken;
        return this;
    }
}