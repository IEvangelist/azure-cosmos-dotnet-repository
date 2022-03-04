// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing;

/// <summary>
/// An event that is stored.
/// </summary>
public interface IPersistedEvent
{
    /// <summary>
    /// The name of the event.
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// The <see cref="DateTime"/> the event occured
    /// </summary>
    DateTime OccuredUtc { get; }
}