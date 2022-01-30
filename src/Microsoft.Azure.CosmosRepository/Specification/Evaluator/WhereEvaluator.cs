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
    public class WhereEvaluator : IEvaluator
    {
        private WhereEvaluator() { }
        /// <summary>
        /// 
        /// </summary>
        public static WhereEvaluator Instance { get; } = new WhereEvaluator();
        /// <summary>
        /// 
        /// </summary>
        public bool IsFilterEvaluator { get; } = true;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public IQueryable<T> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T,TResult> specification)
            where T : IItem
            where TResult : IQueryResult<T>
        {
            foreach (WhereExpressionInfo<T> info in specification.WhereExpressions)
            {
                query = query.Where(info.Filter);
            }

            return query;
        }
    }
}
