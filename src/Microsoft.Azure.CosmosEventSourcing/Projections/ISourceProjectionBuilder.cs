// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

public interface ISourceProjectionBuilder<in TEventSource> where TEventSource : EventSource
{
    ValueTask ProjectAsync(TEventSource sourcedEvent, CancellationToken cancellationToken = default);
}