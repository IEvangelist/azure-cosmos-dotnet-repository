// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <summary>
    /// Interface used for defining specification
    /// </summary>
    public interface ISpecification<T>
        where T : IItem
    {
        /// <summary>
        /// The collection of filters.
        /// </summary>
        IEnumerable<WhereExpressionInfo<T>> WhereExpressions { get; }
        /// <summary>
        /// A collection of expressions used for sorting 
        /// </summary>
        IEnumerable<OrderExpressionInfo<T>> OrderExpressions { get; }

        /// <summary>
        /// Continutation token used for paging in cosmos. If specificed PageNumber will be ignored
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
