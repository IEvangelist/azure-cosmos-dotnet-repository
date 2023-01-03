// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace Microsoft.Azure.CosmosEventSourcing.Aggregates;

/// <summary>
/// This defines an interface that represents an aggregate root.
/// This is responsible for making state changes which in turn add new events.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// New events that have been applied to the <see cref="IAggregateRoot"/>.
    /// </summary>
    IReadOnlyList<DomainEvent> NewEvents { get; }

    /// <summary>
    /// All events that have occured on an <see cref="IAggregateRoot"/>
    /// </summary>
    IReadOnlyList<DomainEvent> Events { get; }

    /// <summary>
    /// An event that is used to provide optimistic concurrency control for the entire <see cref="IAggregateRoot"/>
    /// </summary>
    AtomicEvent AtomicEvent { get; }
}