// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing.Events;

/// <summary>
/// An event that is used to pass between <see cref="IAggregateRoot"/>'s and the <see cref="IEventStore{TEventItem}"/>
/// <remarks>This is used to provide optimistic concurrency control for batches of <see cref="DomainEvent"/>'s.</remarks>
/// </summary>
public record class AtomicEvent : DomainEvent
{
    /// <summary>
    /// Creates an <see cref="AtomicEvent"/>
    /// </summary>
    /// <param name="eventId">The ID of the event</param>
    /// <param name="eTag">The cosmos ETag</param>
    [JsonConstructor]
    public AtomicEvent(string eventId, string eTag)
    {
        EventId = eventId;
        ETag = eTag;
    }

    [JsonIgnore]
    internal string ETag { get; init; }
}