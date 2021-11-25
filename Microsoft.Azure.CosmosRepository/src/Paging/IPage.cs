// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

#nullable enable
using System.Collections.Generic;

namespace Microsoft.Azure.CosmosRepository.Paging
{
    /// <summary>
    /// Represents a page of data from a cosmos query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPage<out T> where T : IItem
    {
        /// <summary>
        /// The total amount items that matched the query.
        /// </summary>
        int Total { get; }

        /// <summary>
        /// The page number.
        /// </summary>
        int Number { get; }

        /// <summary>
        /// The size of the current page.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// The items that are in the current page.
        /// </summary>
        IEnumerable<T> Items { get; }

        /// <summary>
        /// The amount of RU's the given query cost.
        /// </summary>
        double Charge { get; }

        /// <summary>
        /// The continuation token used to load results from a stateless marker.
        /// </summary>
        /// <remarks>This is provided by cosmos DB.</remarks>
        string? Continuation { get; }
    }
}