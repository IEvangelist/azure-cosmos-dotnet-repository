// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator;

internal class SpecificationEvaluator : ISpecificationEvaluator
{
    private static readonly IEnumerable<IEvaluator> Evaluators = new IEvaluator[]
    {
        new WhereEvaluator(),
        new OrderEvaluator(),
        new PagingEvaluator()
    };

    public IQueryable<TItem> GetQuery<TItem, TResult>(
        IQueryable<TItem> query,
        ISpecification<TItem, TResult> specification,
        bool evaluateCriteriaOnly = false)
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        IEnumerable<IEvaluator> evaluators = evaluateCriteriaOnly
            ? Evaluators.Where(e => e.IsFilterEvaluator).ToList()
            : Evaluators;

        return evaluators.Aggregate(query, (current, evaluator) => evaluator.GetQuery(current, specification));
    }

    public TResult GetResult<TItem, TResult>(
        IReadOnlyList<TItem> res,
        ISpecification<TItem, TResult> specification,
        int totalCount,
        double charge,
        string continuationToken)
        where TItem : IItem
        where TResult : IQueryResult<TItem> =>
        specification.PostProcessingAction(res, totalCount, charge, continuationToken);
}