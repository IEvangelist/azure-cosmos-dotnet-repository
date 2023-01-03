// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator;

internal class WhereEvaluator : IEvaluator
{
    public bool IsFilterEvaluator => true;

    public IQueryable<T> GetQuery<T, TResult>(
        IQueryable<T> query,
        ISpecification<T, TResult> specification)
        where T : IItem
        where TResult : IQueryResult<T> =>
        specification.WhereExpressions.Aggregate(query, (current, info) => current.Where(info.Filter));
}