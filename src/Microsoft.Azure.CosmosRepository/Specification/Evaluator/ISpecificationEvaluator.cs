// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator
{
    /// <summary>
    /// The aggregator of <see cref="IEvaluator"/>
    /// Uses multiple evaluators to evaluate specifications
    /// </summary>
    public  interface ISpecificationEvaluator 
    {
        /// <summary>
        /// Returns an IQueryable thats matching all paramaters that are setup in the specification 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">Incoming IQueryable. For retrieve this from the specific datasource provider</param>
        /// <param name="specification">The specification that will be evaluated</param>
        /// <param name="evaluateCriteriaOnly">If only the filter criteria of the specification should be evaluated. Mostly used for Count operations where it's interesting to find the total count for the filter criteria.</param>
        /// <returns></returns>
        IQueryable<TItem> GetQuery<TItem, TResult>(IQueryable<TItem> query, ISpecification<TItem, TResult> specification, bool evaluateCriteriaOnly = false)
            where TItem : IItem
            where TResult : IQueryResult<TItem>;

        /// <summary>
        /// Converts an incoming cosmos query result to the specified  <typeparamref name="TResult"/> 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="res"></param>
        /// <param name="specification"></param>
        /// <param name="totalCount"></param>
        /// <param name="charge"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        TResult GetResult<TItem, TResult>(IReadOnlyList<TItem> res, ISpecification<TItem, TResult> specification, int totalCount, double charge, string continuationToken)
            where TItem : IItem
            where TResult : IQueryResult<TItem>;
    }
}
