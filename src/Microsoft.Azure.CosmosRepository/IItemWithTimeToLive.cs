// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// An implementation of IItem with the additional property "TimeToLive".
/// </summary>
public interface IItemWithTimeToLive : IItem
{
    /// <summary>
    /// The time an item should exist within the container.
    /// <remarks>
    /// When setting this to a positive integer this requires the default TTL at container level to be set to a non-null value.
    /// </remarks>
    /// </summary>
    public TimeSpan? TimeToLive { get; set; }
}