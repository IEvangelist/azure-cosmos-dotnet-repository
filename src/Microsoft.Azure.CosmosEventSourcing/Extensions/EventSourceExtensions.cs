// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

/// <summary>
/// A set of extension methods that can be used on an <see cref="EventSource"/>
/// </summary>
public static class EventSourceExtensions
{
    /// <summary>
    /// Cast's the payload of an <see cref="EventSource"/> to a <see cref="IPersistedEvent"/>
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSource"/> to read the payload from.</param>
    /// <typeparam name="TEvent">The event type the payload will be cast to.</typeparam>
    /// <returns>The TEvent instance.</returns>
    public static TEvent GetEventPayload<TEvent>(this EventSource eventSource)
        where TEvent : IPersistedEvent =>
        (TEvent) eventSource.EventPayload;

    /// <summary>
    /// Cast's the payload of an <see cref="EventSource"/> to a <see cref="IPersistedEvent"/>
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSource"/> to read the payload from.</param>
    /// <typeparam name="TEvent">The event type the payload will be cast to.</typeparam>
    /// <returns>The TEvent instance.</returns>
    /// <remarks>If the event payload cannot be converted to the TEvent type null is returned.</remarks>
    public static TEvent? TryGetEventPayload<TEvent>(this EventSource eventSource)
        where TEvent : class, IPersistedEvent =>
        eventSource.EventPayload switch
        {
            TEvent eventPayload => eventPayload,
            null => null,
            _ => throw new ArgumentOutOfRangeException()
        };
}