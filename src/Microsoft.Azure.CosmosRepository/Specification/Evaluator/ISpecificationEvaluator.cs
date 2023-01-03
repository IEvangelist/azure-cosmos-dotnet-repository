// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator;

internal interface ISpecificationEvaluator
{
    IQueryable<TItem> GetQuery<TItem, TResult>(IQueryable<TItem> query, ISpecification<TItem, TResult> specification, bool evaluateCriteriaOnly = false)
        where TItem : IItem
        where TResult : IQueryResult<TItem>;

    TResult GetResult<TItem, TResult>(IReadOnlyList<TItem> res, ISpecification<TItem, TResult> specification, int totalCount, double charge, string continuationToken)
        where TItem : IItem
        where TResult : IQueryResult<TItem>;
}
