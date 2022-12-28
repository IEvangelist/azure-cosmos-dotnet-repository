// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// An implementation of IItem with the additional property "CreatedTime".
/// </summary>
public interface IItemWithTimeStamps : IItem
{
    /// <summary>
    /// Time stamp of when the item was created.
    /// </summary>
    public DateTime? CreatedTimeUtc { get; set; }

    /// <summary>
    /// Time stamp of the last update.
    /// <remarks>
    /// This value will not be updated on the object passed into create / update methods.
    /// </remarks>
    /// </summary>
    public DateTime LastUpdatedTimeUtc { get; }

    /// <summary>
    /// Epoch time the last update or when the item was created in Cosmos.
    /// <remarks>
    /// Stored as stored in Cosmos DB.
    /// </remarks>
    /// <remarks>
    /// This value will not be updated on the object passed into create / update methods.
    /// </remarks>
    /// </summary>
    public long LastUpdatedTimeRaw { get; }
}