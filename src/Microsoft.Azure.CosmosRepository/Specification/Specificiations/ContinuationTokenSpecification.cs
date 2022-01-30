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
    /// <inheritdoc/>
    public class ContinuationTokenSpecification<TItem> : BaseSpecification<TItem, IPage<TItem>>
        where TItem : IItem
    {
        /// <summary>
        /// Default ctor to set all parameters yourself
        /// </summary>
        public ContinuationTokenSpecification()
        {
            UseContinutationToken = true;   
        }
        /// <summary>
        /// Ctor for specifiying the token and page size
        /// </summary>
        /// <param name="continutationToken"></param>
        /// <param name="pageSize"></param>
        public ContinuationTokenSpecification(string continutationToken, int pageSize)
        {
            UseContinutationToken = true;
            Query.ContinutationToken(continutationToken);
            Query.PageSize(pageSize);
        }
        /// <summary>
        /// When scrolling through multiple pages reuse the same specification and use this method to update the continuation token
        /// </summary>
        /// <param name="continuationToken"></param>
        public void UpdateContinutationToken(string continuationToken)
        {
            Query.ContinutationToken(continuationToken);
        }

        /// <inheritdoc/>
        public override IPage<TItem> PostProcessingAction(IReadOnlyList<TItem> queryResult, int totalCount, double charge, string continuationToken)
        {
            return new Page<TItem>(totalCount, PageSize, queryResult, charge, continuationToken);
        }
    }
}
