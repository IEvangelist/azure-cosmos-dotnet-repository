// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator
{
    /// <inheritdoc/>
    public class SpecificationEvaluator : ISpecificationEvaluator
    {
        IEnumerable<IEvaluator> evaluators;
        /// <summary>
        /// default ctor
        /// </summary>
        public SpecificationEvaluator()
        {
            evaluators = new IEvaluator[]
            {
                WhereEvaluator.Instance,
                OrderEvaluator.Instance,
                PagingEvaluator.Instance
            };
        }
        /// <inheritdoc/>
        public IQueryable<TItem> GetQuery<TItem, TResult>(IQueryable<TItem> query, ISpecification<TItem, TResult> specification, bool evaluateCriteriaOnly = false)
            where TItem : IItem
            where TResult : IQueryResult<TItem>
        {
            IEnumerable<IEvaluator> evaluators = evaluateCriteriaOnly ? this.evaluators.Where(e => e.IsFilterEvaluator).ToList() : this.evaluators;

            foreach(IEvaluator evaluator in evaluators)
            {
                query = evaluator.GetQuery(query, specification);
            }

            return query;
           
        }
        /// <inheritdoc/>
        public TResult GetResult<TItem, TResult>(IReadOnlyList<TItem> res, ISpecification<TItem, TResult> specification, int totalCount, double charge, string continuationToken)
            where TItem : IItem
            where TResult : IQueryResult<TItem>
        {
            return specification.PostProcessingAction(res, totalCount, charge, continuationToken);
        }

    }
}
