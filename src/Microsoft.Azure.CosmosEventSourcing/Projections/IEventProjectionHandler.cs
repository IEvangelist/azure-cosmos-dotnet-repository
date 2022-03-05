// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

/// <summary>
/// Allows a projection to be built from a specific <see cref="IPersistedEvent"/> defined within an <see cref="EventSource"/>
/// </summary>
/// <typeparam name="TEvent">The type of <see cref="IPersistedEvent"/></typeparam>
/// <typeparam name="TEventSource">The <see cref="EventSource"/>The event was part of</typeparam>
public interface IEventProjectionHandler<in TEvent, in TEventSource>
    where TEvent : IPersistedEvent
    where TEventSource : EventSource
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
        TEventSource eventSource,
        CancellationToken cancellationToken = default);
}