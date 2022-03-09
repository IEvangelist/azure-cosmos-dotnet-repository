// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing;

/// <summary>
/// A record the represents an event stored in an <see cref="IEventStore{TEventItem}"/>
/// </summary>
public abstract class EventItem : FullItem
{
    /// <summary>
    /// The payload of the event to be stored.
    /// </summary>
    [JsonConverter(typeof(DomainEventConverter))]
    public IDomainEvent EventPayload { get; set; } = null!;

    /// <summary>
    /// The value used to partition the event.
    /// </summary>
    public string PartitionKey { get; set; } = null!;

    /// <inheritdoc />
    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    /// <summary>
    /// The name of the event stored.
    /// </summary>
    public string EventName { get; set; } = null!;

    /// <summary>
    /// Creates an event item.
    /// </summary>
    /// <param name="eventPayload">The payload of the event.</param>
    /// <param name="partitionKey">The value to use as the partition key for the event.</param>
    /// <exception cref="ArgumentNullException">Occurs when the partition key value is a empty string or null.</exception>
    protected EventItem(
        IDomainEvent eventPayload,
        string partitionKey)
    {
        if (string.IsNullOrWhiteSpace(partitionKey))
        {
            throw new ArgumentNullException(nameof(partitionKey), "The partition key must be provided");
        }

        EventPayload = eventPayload;
        EventName = eventPayload.EventName;
        PartitionKey = partitionKey;
    }

    /// <summary>
    /// Creates an event item.
    /// </summary>
    protected EventItem()
    {

    }
}