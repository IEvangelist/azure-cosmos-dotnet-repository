// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

namespace Microsoft.Azure.CosmosEventSourcing.ChangeFeed;

internal class DefaultEventSourcingProvider : IChangeFeedContainerProcessorProvider
{
    private readonly IEnumerable<IEventSourcingProcessor> _processors;

    public DefaultEventSourcingProvider(IEnumerable<IEventSourcingProcessor> processors) =>
        _processors = processors;

    public IEnumerable<IContainerChangeFeedProcessor> GetProcessors() =>
        _processors;
}