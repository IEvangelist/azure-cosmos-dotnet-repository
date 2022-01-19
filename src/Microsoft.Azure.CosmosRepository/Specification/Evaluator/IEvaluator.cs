// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator
{
    /// <summary>
    /// 
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
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        IQueryable<T> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification)
            where T : IItem
            where TResult : IQueryResult<T>;

    }
}
