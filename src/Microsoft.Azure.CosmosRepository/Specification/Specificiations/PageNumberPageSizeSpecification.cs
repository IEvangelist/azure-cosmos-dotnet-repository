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
    /// A specification used for the Offset and Limit pattern
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class OffsetByPageNumberSpecification<TItem> : BaseSpecification<TItem, IPageQueryResult<TItem>>
        where TItem : IItem
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public OffsetByPageNumberSpecification() { }

        /// <summary>
        /// Helper ctor to set pagenumber and pagesize
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public OffsetByPageNumberSpecification(int pageNumber, int pageSize)
        {
            Query.PageSize(pageSize);
            Query.PageNumber(pageNumber);
        }

        /// <summary>
        /// Update the specification to get the next page of the result
        /// </summary>
        public void NextPage()
        {
            Query.PageNumber(PageNumber.Value + 1);
        }

        /// <summary>
        /// Update the specification to get the previous page of the result
        /// </summary>
        public void PreviousPage()
        {
            Query.PageNumber(PageNumber.Value - 1);
        }

        /// <inheritdoc/>
        public override IPageQueryResult<TItem> PostProcessingAction(IReadOnlyList<TItem> queryResult, int totalCount, double charge, string continuationToken)
        {
            return new PageQueryResult<TItem>(
                totalCount,
                PageNumber,
                PageSize,
                queryResult,
                charge);
        }
    }
}
