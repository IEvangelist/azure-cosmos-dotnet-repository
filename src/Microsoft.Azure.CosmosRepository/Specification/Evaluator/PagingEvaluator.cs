// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator;

internal class PagingEvaluator : IEvaluator
{
    public bool IsFilterEvaluator => false;

    public IQueryable<TItem> GetQuery<TItem, TResult>(
        IQueryable<TItem> query,
        ISpecification<TItem, TResult> specification)
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        if (specification.UseContinuationToken)
        {
            return query;
        }

        if (specification.PageNumber.HasValue && specification.PageNumber != 0)
        {
            query = query.Skip(specification.PageSize * (specification.PageNumber.Value - 1));
        }

        query = query.Take(specification.PageSize);

        return query;
    }
}