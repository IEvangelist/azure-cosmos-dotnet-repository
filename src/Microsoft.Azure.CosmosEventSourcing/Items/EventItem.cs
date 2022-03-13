// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing.Items;

/// <summary>
/// A record the represents an event stored in an <see cref="IEventStore{TEventItem}"/>
/// </summary>
public abstract class EventItem : IItemWithEtag, IItemWithTimeToLive
{
    /// <summary>
    /// The payload of the event to be stored.
    /// </summary>
    [JsonConverter(typeof(DomainEventConverter))]
    public IDomainEvent EventPayload
    {
        get => _eventPayload;
        set
        {
            if (value is AtomicEvent atomicEvent)
            {
                Id = atomicEvent.Id.ToString();
            }

            _eventPayload = value;
        }
    }

    /// <inheritdoc />
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <inheritdoc />
    public string Type { get; set; } = null!;

    /// <summary>
    /// The value used to partition the event.
    /// </summary>
    public string PartitionKey { get; set; } = null!;

    /// <summary>
    /// The name of the event stored.
    /// </summary>
    public string EventName { get; set; } = null!;

    /// <inheritdoc />
    [JsonProperty("_etag")]
    public string Etag { get; private set; } = null!;

    /// <inheritdoc />
    public TimeSpan? TimeToLive
    {
        get => _timeToLive.HasValue ? TimeSpan.FromSeconds(_timeToLive.Value) : null;
        set => _timeToLive = (int?) value?.TotalSeconds;
    }

    [JsonProperty("ttl", NullValueHandling = NullValueHandling.Ignore)]
    private int? _timeToLive;

    private IDomainEvent _eventPayload = null!;

    /// <summary>
    /// Creates an event item.
    /// </summary>
    /// <param name="eventPayload">The payload of the event.</param>
    /// <param name="partitionKey">The value to use as the partition key for the event.</param>
    /// <param name="etag"></param>
    /// <exception cref="ArgumentNullException">Occurs when the partition key value is a empty string or null.</exception>
    protected EventItem(
        IDomainEvent eventPayload,
        string partitionKey,
        string etag)
    {
        if (string.IsNullOrWhiteSpace(partitionKey))
        {
            throw new ArgumentNullException(nameof(partitionKey), "The partition key must be provided");
        }

        if (eventPayload is AtomicEvent atomicEvent)
        {
            Id = atomicEvent.Id.ToString();
        }

        EventPayload = eventPayload;
        EventName = eventPayload.EventName;
        PartitionKey = partitionKey;
        Type = GetType().Name;
        Etag = etag;
    }

    /// <summary>
    /// Creates an <see cref="EventItem"/>
    /// </summary>
    public EventItem()
    {

    }
}