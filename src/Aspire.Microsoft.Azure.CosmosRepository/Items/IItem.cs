// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Aspire.Microsoft.Azure.CosmosRepository.Items;

/// <summary>
/// The base interface used for all repository object or object graphs.
/// </summary>
public interface IItem
{
    /// <summary>
    /// Gets or sets the item's globally unique identifier.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Gets or sets the item's type name.
    /// </summary>
    string Type { get; set; }
}
