// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// The base interface used for all repository object or object graphs.
/// </summary>
public interface IItemWithEtag : IItem
{
    /// <summary>
    /// Etag for the item which was set by Cosmos the last time the item was updated. This string is used for the relevant operations when specified.
    /// </summary>
    string? Etag { get; }
}