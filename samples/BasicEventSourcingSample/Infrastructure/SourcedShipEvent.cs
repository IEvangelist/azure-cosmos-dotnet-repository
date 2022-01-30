// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.


using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Infrastructure;

public class SourcedShipEvent : SourcedEvent
{
    public SourcedShipEvent(IPersistedEvent eventPayload, string shipName)
        : base(eventPayload, shipName)
    {
    }

    public SourcedShipEvent()
    {

    }
}