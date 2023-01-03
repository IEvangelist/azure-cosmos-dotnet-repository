// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#nullable enable

namespace Microsoft.Azure.CosmosRepository.Paging;

/// <summary>
/// Represents a page of data from a cosmos query
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPageQueryResult<out T> : IPage<T> where T : IItem
{
    /// <summary>
    /// The total amount pages that matched the query.
    /// </summary>
    int? TotalPages { get; }

    /// <summary>
    /// The total amount items that matched the query.
    /// </summary>
    int? PageNumber { get; }

    /// <summary>
    /// Gets a value indicating whether the are next pages.
    /// </summary>
    bool? HasNextPage { get; }

    /// <summary>
    /// Gets a value indicating whether the are previous pages.
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    /// The previous page number.
    /// </summary>
    int PreviousPageNumber { get; }

    /// <summary>
    /// The next page number.
    /// </summary>
    int? NextPageNumber { get; }
}