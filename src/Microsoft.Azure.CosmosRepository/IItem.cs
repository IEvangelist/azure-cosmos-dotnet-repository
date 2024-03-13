// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

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

    ///// <summary>
    ///// Gets the item's PartitionKey. This string is used to instantiate the <c>Cosmos.PartitionKey</c> struct.
    ///// </summary>
    string PartitionKey { get; }

    /// <summary>
    /// Gets the item PartitionKeys. This string array is used to instantiate the <c>Cosmos.PartitionKeys</c> struct.
    /// </summary>
    IEnumerable<string> PartitionKeys { get; }
}
