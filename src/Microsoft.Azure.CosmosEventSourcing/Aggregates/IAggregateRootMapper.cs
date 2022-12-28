// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Aggregates;

/// <summary>
/// Maps from events to aggregates and vice versa.
/// </summary>
public interface IAggregateRootMapper<TAggregateRoot, TEventItem>
    where TAggregateRoot : IAggregateRoot
    where TEventItem : EventItem
{
    /// <summary>
    /// Maps an <see cref="IAggregateRoot"/> to a set of <see cref="EventItem"/>'s
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="IAggregateRoot"/> to map.</param>
    IEnumerable<TEventItem> MapFrom(TAggregateRoot aggregateRoot);

    /// <summary>
    /// Maps a collection of <see cref="DomainEvent"/>'s
    /// </summary>
    TAggregateRoot MapTo(IEnumerable<TEventItem> events);
}