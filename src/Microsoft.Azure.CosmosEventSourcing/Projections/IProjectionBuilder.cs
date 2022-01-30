// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

public interface IProjectionBuilder<in TEvent> where TEvent : IPersistedEvent
{
    ValueTask ProjectAsync(TEvent persistedEvent, CancellationToken cancellationToken = default);
}