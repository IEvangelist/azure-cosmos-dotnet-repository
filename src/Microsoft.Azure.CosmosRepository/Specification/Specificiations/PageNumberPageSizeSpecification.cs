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
    public class OffsetByPageNumberSpecification<T> : BaseSpecification<T, IPageQueryResult<T>>
        where T : IItem
    {
        /// <summary>
        /// 
        /// </summary>
        public OffsetByPageNumberSpecification() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public OffsetByPageNumberSpecification(int pageNumber, int pageSize)
        {
            Query.PageSize(pageSize);
            Query.PageNumber(pageNumber);
        }
        /// <summary>
        /// 
        /// </summary>
        public void NextPage()
        {
            Query.PageNumber(PageNumber.Value + 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResult"></param>
        /// <param name="totalCount"></param>
        /// <param name="charge"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        public override IPageQueryResult<T> PostProcessingAction(IReadOnlyList<T> queryResult, int totalCount, double charge, string continuationToken)
        {
            return new PageQueryResult<T>(
                totalCount,
                PageNumber,
                PageSize,
                queryResult,
                charge);
        }
    }
}
