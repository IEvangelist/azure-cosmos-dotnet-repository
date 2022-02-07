// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.


using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipEventSource : EventSource
{
    public ShipEventSource(IPersistedEvent eventPayload, string shipName)
        : base(eventPayload, shipName)
    {
    }

    public ShipEventSource()
    {
    }
}