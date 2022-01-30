// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Infrastructure;
using Microsoft.Azure.CosmosEventSourcing.Projections;

namespace BasicEventSourcingSample.Projections;

public class SourcedShipEventsProjectionBuilder : ISourceProjectionBuilder<SourcedShipEvent>
{
    public ValueTask ProjectAsync(SourcedShipEvent sourcedEvent, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}