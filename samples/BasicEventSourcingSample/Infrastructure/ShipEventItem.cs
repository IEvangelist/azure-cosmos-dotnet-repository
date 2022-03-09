// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipEventItem : DefaultEventItem
{
    public ShipEventItem(DefaultDomainEvent eventPayload, string shipName)
        : base(eventPayload, shipName)
    {
    }

    public ShipEventItem()
    {
    }
}