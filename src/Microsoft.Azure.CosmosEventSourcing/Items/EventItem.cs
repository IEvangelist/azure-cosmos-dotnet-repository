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

    [JsonProperty("ttl", NullValueHandling = NullValueHandling.Ignore)]
    private int? _timeToLive;

    private DomainEvent _domainEvent = null!;

    /// <summary>
    /// The payload of the event to be stored.
    /// </summary>
    [JsonConverter(typeof(DomainEventConverter))]
    public DomainEvent DomainEvent
    {
        get => _domainEvent;
        set
        {
            Id = value.EventId;
            EventName = value.EventName;
            _domainEvent = value;
        }
    }

    /// <inheritdoc />
    public string Id { get; set; }

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

    /// <summary>
    /// Creates an <see cref="EventItem"/>
    /// </summary>
    protected EventItem()
    {
        Type = GetType().Name;
        Id = Guid.NewGuid().ToString();
    }
}