// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing.Items;

/// <summary>
/// A default event item which stores <see cref="DomainEventPayload"/>'s
/// </summary>
public class DefaultEventItem : EventItem
{
    /// <summary>
    /// Creates an <see cref="DefaultEventItem"/>
    /// </summary>
    /// <param name="eventPayload">The payload of the event.</param>
    /// <param name="partitionKey">The partitionKey of the event.</param>
    protected DefaultEventItem(
        IDomainEvent eventPayload,
        string partitionKey) :
        base(eventPayload, partitionKey)
    {
    }

    /// <summary>
    /// Converts an <see cref="IDomainEvent"/> to an <see cref="DomainEvent"/>
    /// </summary>
    [JsonIgnore]
    public DomainEvent DomainEventPayload =>
        (DomainEvent) EventPayload;

    /// <summary>
    /// Creates an <see cref="DefaultEventItem"/>
    /// </summary>
    protected DefaultEventItem()
    {
    }
}