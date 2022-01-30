// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.


using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Infrastructure;

public class SourcedShipEvent : SourcedEvent
{
    public SourcedShipEvent(IPersistedEvent @event, string shipName)
        : base(@event, shipName)
    {
    }

    public SourcedShipEvent()
    {

    }
}