// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <summary>
/// Whether to (subsequently) sort ascending or descending.
/// </summary>
public enum OrderTypeEnum
{
    /// <summary>
    /// Order by ascending
    /// </summary>
    OrderBy = 1,
    /// <summary>
    /// Order by descending
    /// </summary>
    OrderByDescending = 2,
    /// <summary>
    /// ThenBy must be chained after another other by expression
    /// </summary>
    ThenBy = 3,
    /// <summary>
    /// ThenByDescending must be chained after another other by expression
    /// </summary>
    ThenByDescending = 4
}
