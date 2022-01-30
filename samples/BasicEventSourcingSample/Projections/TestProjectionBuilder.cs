// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Core;
using Microsoft.Azure.CosmosEventSourcing.Projections;

namespace BasicEventSourcingSample.Projections;

public class TestProjectionBuilder : IProjectionBuilder<ShipEvents.TestShipEvent>
{
    public ValueTask ProjectAsync(ShipEvents.TestShipEvent persistedEvent, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }
}