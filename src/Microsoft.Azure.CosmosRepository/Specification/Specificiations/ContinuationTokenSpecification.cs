// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContinuationTokenSpecification<T> : BaseSpecification<T, IPage<T>>
        where T : IItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="continutationToken"></param>
        /// <param name="pageSize"></param>
        public ContinuationTokenSpecification(string continutationToken, int pageSize)
        {
            UseContinutationToken = true;
            Query.ContinutationToken(continutationToken);
            Query.PageSize(pageSize);
        }

        internal void UpdateContinutationToken(string continuationToken)
        {
            Query.ContinutationToken(continuationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResult"></param>
        /// <param name="totalCount"></param>
        /// <param name="charge"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IPage<T> PostProcessingAction(IReadOnlyList<T> queryResult, int totalCount, double charge, string continuationToken)
        {
            return new Page<T>(totalCount, PageSize, queryResult, charge, continuationToken);
        }
    }
}
