// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

namespace Microsoft.Azure.CosmosEventSourcing.ChangeFeed;

public class EventSourcingProvider : IChangeFeedContainerProcessorProvider
{
    private readonly IEnumerable<IEventSourcingProcessor> _processors;

    public EventSourcingProvider(IEnumerable<IEventSourcingProcessor> processors) =>
        _processors = processors;

    public IEnumerable<IContainerChangeFeedProcessor> GetProcessors() =>
        _processors;
}