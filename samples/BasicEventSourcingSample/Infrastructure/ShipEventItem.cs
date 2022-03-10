// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipEventItem : DefaultEventItem
{
    public ShipEventItem(
        IDomainEvent eventPayload,
        string partitionKey)
        : base(eventPayload, partitionKey)
    {
    }
}