// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListSpecification<T> : BaseSpecification<T, IQueryResult<T>>
        where T : IItem

    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResult"></param>
        /// <param name="totalCount"></param>
        /// <param name="charge"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        public override IQueryResult<T> PostProcessingAction(IReadOnlyList<T> queryResult, int totalCount, double charge, string continuationToken)
        {
            return new QueryResult<T>(queryResult, charge);
        }
    }
}
