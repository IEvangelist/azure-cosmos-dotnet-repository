// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Infrastructure;
using Microsoft.Azure.CosmosEventSourcing.Projections;

namespace BasicEventSourcingSample.Projections;

public class SourcedShipEventsProjectionBuilder : ISourceProjectionBuilder<SourcedShipEvent>
{
    private readonly ILogger<SourcedShipEventsProjectionBuilder> _logger;

    public SourcedShipEventsProjectionBuilder(ILogger<SourcedShipEventsProjectionBuilder> logger) =>
        _logger = logger;

    public ValueTask ProjectAsync(SourcedShipEvent sourcedEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sourced event received {EventName}", sourcedEvent.EventName);
        return ValueTask.CompletedTask;
    }
}