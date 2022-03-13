// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Newtonsoft.Json;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipEventItem : EventItem
{
    public ShipEventItem(
        DomainEvent domainEvent,
        string partitionKey)
    {
        DomainEvent = domainEvent;
        PartitionKey = partitionKey;
    }
}