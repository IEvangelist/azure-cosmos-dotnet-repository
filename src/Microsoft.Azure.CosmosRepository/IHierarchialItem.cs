// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// The base interface used for all repository hierarchical object or object graphs.
/// </summary>
public interface IHierarchialItem : IItem
{
    /// <summary>
    /// Gets the item's PartitionKeys. This list is used to instantiate the <c>Cosmos.PartitionKeys</c> struct.
    /// </summary>
    IEnumerable<string> PartitionKeys { get; }
}
