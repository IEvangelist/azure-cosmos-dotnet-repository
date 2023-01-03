// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

/// <summary>
/// A set of extension methods that can be used on an <see cref="EventItem"/>
/// </summary>
public static class EventItemExtensions
{
    /// <summary>
    /// Cast's the payload of an <see cref="EventItem"/> to a <see cref="IDomainEvent"/>
    /// </summary>
    /// <param name="eventItem">The <see cref="EventItem"/> to read the payload from.</param>
    /// <typeparam name="TEvent">The event type the payload will be cast to.</typeparam>
    /// <returns>The TEvent instance.</returns>
    public static TEvent GetEventPayload<TEvent>(this EventItem eventItem)
        where TEvent : DomainEvent =>
        (TEvent)eventItem.DomainEvent;

    /// <summary>
    /// Cast's the payload of an <see cref="EventItem"/> to a <see cref="IDomainEvent"/>
    /// </summary>
    /// <param name="eventItem">The <see cref="EventItem"/> to read the payload from.</param>
    /// <typeparam name="TEvent">The event type the payload will be cast to.</typeparam>
    /// <returns>The TEvent instance.</returns>
    /// <remarks>If the event payload cannot be converted to the TEvent type null is returned.</remarks>
    public static TEvent? TryGetEventPayload<TEvent>(this EventItem eventItem)
        where TEvent : DomainEvent =>
        eventItem.DomainEvent switch
        {
            TEvent eventPayload => eventPayload,
            null => null,
            _ => throw new ArgumentOutOfRangeException()
        };
}