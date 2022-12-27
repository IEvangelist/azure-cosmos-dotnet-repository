// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed;

class DefaultChangeFeedService : IChangeFeedService
{
    private readonly IEnumerable<IChangeFeedContainerProcessorProvider> _changeFeedContainerProcessorProvider;
    private IEnumerable<IContainerChangeFeedProcessor> _processors;

    public DefaultChangeFeedService(IEnumerable<IChangeFeedContainerProcessorProvider> changeFeedContainerProcessorProvider)
    {
        _processors = new List<IContainerChangeFeedProcessor>();
        _changeFeedContainerProcessorProvider = changeFeedContainerProcessorProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _processors = _changeFeedContainerProcessorProvider.SelectMany(x => x.GetProcessors());

        cancellationToken.Register(() => StopAsync().Wait(TimeSpan.FromSeconds(5)));

        return Task.WhenAll(
            _processors.Select(
                x => x.StartAsync()));
    }

    public async Task StopAsync()
    {
        foreach (IContainerChangeFeedProcessor processor in _processors)
        {
            if (processor is not null)
            {
                await processor.StopAsync();
            }
        }
    }
}