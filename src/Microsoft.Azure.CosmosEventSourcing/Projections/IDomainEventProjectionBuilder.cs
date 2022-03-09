// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

/// <summary>
/// Allows a projection to be built from a specific <see cref="IDomainEvent"/> defined within an <see cref="EventItem"/>
/// </summary>
/// <typeparam name="TEvent">The type of <see cref="IDomainEvent"/></typeparam>
/// <typeparam name="TEventItem">The <see cref="EventItem"/>The event was part of</typeparam>
public interface IDomainEventProjectionBuilder<in TEvent, in TEventItem>
    where TEvent : IDomainEvent
    where TEventItem : EventItem
{
    /// <summary>
    /// A method to process a new event after it has been saved into Cosmos.
    /// </summary>
    /// <remarks>This is invoked off the back fo the change feed processor library.</remarks>
    /// <param name="persistedEvent">The event that was written.</param>
    /// <param name="eventSource">The event source with all it's properties</param>
    /// <param name="cancellationToken">A token used to cancel the async operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the async operation</returns>
    ValueTask HandleAsync(
        TEvent persistedEvent,
        TEventItem eventSource,
        CancellationToken cancellationToken = default);
}