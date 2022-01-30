// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <summary>
    /// A specification used for getting all results in a <see cref="QueryResult{T}"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class ListSpecification<TItem> : BaseSpecification<TItem, IQueryResult<TItem>>
        where TItem : IItem
    {
        /// <inheritdoc/>
        public override IQueryResult<TItem> PostProcessingAction(IReadOnlyList<TItem> queryResult, int totalCount, double charge, string continuationToken)
        {
            return new QueryResult<TItem>(queryResult, charge);
        }
    }
}
