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
        /// 
        /// </summary>
        bool IsCriteriaEvaluator { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : IItem;

    }
}
