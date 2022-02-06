// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

public interface IEventProjectionHandler<in TEvent> where TEvent : IPersistedEvent
{
    ValueTask HandleAsync(TEvent persistedEvent, CancellationToken cancellationToken = default);
}