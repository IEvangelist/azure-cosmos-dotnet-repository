// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing;

public abstract class SourcedEvent : FullItem
{
    [JsonConverter(typeof(PersistedEventConverter))]
    public IPersistedEvent Event { get; set; } = null!;

    public string PartitionKey { get; set; } = null!;

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public string EventName { get; set; } = null!;

    protected SourcedEvent(IPersistedEvent @event, string partitionKey)
    {
        if (string.IsNullOrWhiteSpace(partitionKey))
        {
            throw new ArgumentNullException(nameof(partitionKey), "The partition key must be provided");
        }

        Event = @event;
        EventName = @event.EventName;
        PartitionKey = partitionKey;
    }

    public SourcedEvent()
    {

    }
}