// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Aggregates;

/// <summary>
/// Maps from events to aggregates and vice versa.
/// </summary>
public interface IAggregateRootMapper<TAggregateRoot>
    where TAggregateRoot : IAggregateRoot
{
    /// <summary>
    /// Maps an <see cref="IAggregateRoot"/> to a set of <see cref="EventItem"/>'s
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="IAggregateRoot"/> to map.</param>
    IEnumerable<EventItem> MapFrom(TAggregateRoot aggregateRoot);

    /// <summary>
    /// Maps a collection of <see cref="DomainEvent"/>'s
    /// </summary>
    TAggregateRoot MapTo(IEnumerable<DomainEvent> domainEvents);
}