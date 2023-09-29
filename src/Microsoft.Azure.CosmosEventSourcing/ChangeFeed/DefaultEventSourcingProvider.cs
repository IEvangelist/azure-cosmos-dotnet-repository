// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

namespace Microsoft.Azure.CosmosEventSourcing.ChangeFeed;

internal class DefaultEventSourcingProvider(IEnumerable<IEventSourcingProcessor> processors) : IChangeFeedContainerProcessorProvider
{
    public IEnumerable<IContainerChangeFeedProcessor> GetProcessors() =>
        processors;
}