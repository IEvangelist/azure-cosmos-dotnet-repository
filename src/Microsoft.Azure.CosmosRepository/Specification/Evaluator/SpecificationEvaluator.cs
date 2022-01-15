// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator
{
    class SpecificationEvaluator : ISpecificationEvaluator
    {
        IEnumerable<IEvaluator> evaluators;

        public SpecificationEvaluator()
        {
            evaluators = new IEvaluator[]
            {
                WhereEvaluator.Instance,
                OrderEvaluator.Instance,
                PagingEvaluator.Instance
            };
        }

        public IQueryable<T> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification, bool evaluateCriteriaOnly = false)
            where T : IItem
            where TResult : IQueryResult<T>
        {
            IEnumerable<IEvaluator> evaluators = evaluateCriteriaOnly ? this.evaluators.Where(e => e.IsCriteriaEvaluator).ToList() : this.evaluators;

            foreach(IEvaluator evaluator in evaluators)
            {
                query = evaluator.GetQuery(query, specification);
            }

            return query;
           
        }

        public TResult GetResult<T, TResult>(IReadOnlyList<T> res, ISpecification<T, TResult> specification, int totalCount, double charge, string continuationToken)
            where T : IItem
            where TResult : IQueryResult<T>
        {
            return specification.PostProcessingAction(res, totalCount, charge, continuationToken);
        }

    }
}
