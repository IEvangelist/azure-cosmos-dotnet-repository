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
    public class PagingEvaluator : IEvaluator
    {
        private PagingEvaluator() { }
        /// <summary>
        /// 
        /// </summary>
        public static PagingEvaluator Instance { get; } = new PagingEvaluator();
        /// <summary>
        /// 
        /// </summary>
        public bool IsCriteriaEvaluator { get; } = false;
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
            if (specification.UseContinutationToken)
            {
                return query;
            }          

            if(specification.PageNumber.HasValue && specification.PageNumber != 0)
            {
                query = query.Skip(specification.PageSize * (specification.PageNumber.Value - 1));
            }

            query = query.Take(specification.PageSize);

            return query;
        }
    }
}
