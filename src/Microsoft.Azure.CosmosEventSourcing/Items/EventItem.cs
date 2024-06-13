// Copyright (c) David Pine. All rights reserved.
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
    private string? _etag;

    /// <summary>
    /// The payload of the event to be stored.
    /// </summary>
    [JsonConverter(typeof(DomainEventConverter))]
    public DomainEvent DomainEvent
    {
        get
        {
            if (_domainEvent is AtomicEvent atomicEvent && string.IsNullOrWhiteSpace(atomicEvent.ETag))
            {
                // Use the ETag from the Item to pass back with the domain event to the aggregate.
                _domainEvent = atomicEvent with
                {
                    ETag = Etag ?? string.Empty,
                    EventId = Id,
                };
            }

            return _domainEvent;
        }
        init
        {
            Id = value.EventId;
            EventName = value.EventName;
            _domainEvent = value;
        }
    }

    /// <inheritdoc />
    public string Id { get; set; }

    /// <inheritdoc />
    public string Type { get; set; }

    /// <summary>
    /// The value used to partition the event.
    /// </summary>
    public string PartitionKey { get; set; } = null!;

    /// <summary>
    /// The values used to partition the event.
    /// </summary>
    public IEnumerable<string> PartitionKeys { get; set; } = null!;

    /// <summary>
    /// The name of the event stored.
    /// </summary>
    public string EventName { get; set; } = null!;

    /// <inheritdoc />
    [JsonProperty("_etag")]
    public string? Etag
    {
        get
        {
            if (_etag is null && _domainEvent is AtomicEvent atomicEvent && !string.IsNullOrWhiteSpace(atomicEvent.ETag))
            {
                // If the item is being created to be saved back then use the ETag from the AtomicEvent.
                return atomicEvent.ETag;
            }

            return _etag;
        }
        private set => _etag = value;
    }

    /// <inheritdoc />
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TimeSpan? TimeToLive
    {
        get => _timeToLive.HasValue ? TimeSpan.FromSeconds(_timeToLive.Value) : null;
        set => _timeToLive = (int?)value?.TotalSeconds;
    }

    /// <summary>
    /// An ID that correlates a request/scope with an event.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Creates an <see cref="EventItem"/>
    /// </summary>
    protected EventItem()
    {
        Type = GetType().Name;
        Id = Guid.NewGuid().ToString();
    }
}