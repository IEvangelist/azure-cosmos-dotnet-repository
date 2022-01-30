// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public class ShipEvents
{
    public record TestShipEvent(string Id) : IPersistedEvent
    {
        public string EventName => nameof(TestShipEvent);
    }
}