// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipEventItem : EventItem
{
    public ShipEventItem(IDomainEvent eventPayload, string shipName)
        : base(eventPayload, shipName)
    {
    }

    public ShipEventItem()
    {
    }
}