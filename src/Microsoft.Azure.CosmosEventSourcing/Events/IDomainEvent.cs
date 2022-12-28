// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Events;

/// <summary>
/// An event that occurs inside a domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// A unique ID to identify this event.
    /// </summary>
    string EventId { get; }

    /// <summary>
    /// The name of the event.
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// The sequence number in which the event occured.
    /// </summary>
    int Sequence { get; }

    /// <summary>
    /// The <see cref="DateTime"/> that this event occured
    /// </summary>
    DateTime OccuredUtc { get; }
}