// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <summary>
    /// Interface used for defining specification
    /// </summary>
    public interface ISpecification<T, out TResult>
        where T : IItem
        where TResult : IQueryResult<T>
    {
        /// <summary>
        /// The collection of filter expressions used for filtering queries.
        /// </summary>
        IReadOnlyList<WhereExpressionInfo<T>> WhereExpressions { get; }
        /// <summary>
        /// A collection of expressions used for sorting.
        /// </summary>
        IReadOnlyList<OrderExpressionInfo<T>> OrderExpressions { get; }
        /// <summary>
        /// Processing for updating the query result before returning it from the repository. Given the methods input it should generate the specified TResult />
        /// </summary>
        TResult PostProcessingAction(IReadOnlyList<T> queryResult, int totalCount, double charge, string continuationToken);

        /// <summary>
        /// Continutation token used for paging in cosmos. Must set <see cref="UseContinutationToken"/> for continuation token to be applicable
        /// </summary>
        string ContinutationToken { get; }
        /// <summary>
        /// Select which page shoud be selected in the paginated result
        /// </summary>
        int? PageNumber { get; }
        /// <summary>
        /// Paginate results, selects how many results should be returned
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Use contiutation token instead of pagenumber
        /// </summary>
        bool UseContinutationToken { get; }


    }
}
