// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator
{
    /// <summary>
    /// Evaluates specific parts on a specification
    /// All evaluators should have a single purpose
    /// </summary>
    public interface IEvaluator
    {
        /// <summary>
        /// Defines if the evaluator is used for filtering the results. Evaluators that are not used for filtering are better to ignore when doing things like a Count.
        /// </summary>
        bool IsFilterEvaluator { get; }
        /// <summary>
        /// Evaluates something in the specification and then adds it to an <see cref="IQueryable"/> and returns the updated <see cref="IQueryable"/>
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        IQueryable<TItem> GetQuery<TItem, TResult>(IQueryable<TItem> query, ISpecification<TItem, TResult> specification)
            where TItem : IItem
            where TResult : IQueryResult<TItem>;

    }
}
