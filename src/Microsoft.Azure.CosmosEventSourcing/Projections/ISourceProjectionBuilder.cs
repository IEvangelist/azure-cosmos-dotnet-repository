// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

public interface ISourceProjectionBuilder<in TSourceEvent> where TSourceEvent : SourcedEvent
{
    ValueTask ProjectAsync(TSourceEvent sourcedEvent, CancellationToken cancellationToken = default);
}