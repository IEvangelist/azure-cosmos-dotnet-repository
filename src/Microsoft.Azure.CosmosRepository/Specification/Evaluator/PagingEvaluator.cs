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
        /// <param name="query"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : IItem
        {
            if(specification.UseContinutationToken)
            {
                return query;
            }          

            if(specification.PageNumber.HasValue && specification.PageNumber != 0)
            {
                query = query.Skip(specification.PageSize * (specification.PageNumber.Value - 1));
            }

            if(specification == null)
            {
                query = query.Take(specification.PageSize);
            }

            return query;
        }
    }
}
