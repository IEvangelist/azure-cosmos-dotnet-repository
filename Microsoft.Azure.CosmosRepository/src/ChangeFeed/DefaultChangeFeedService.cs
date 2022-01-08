// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Processors;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed
{
    class DefaultChangeFeedService : IChangeFeedService
    {
        private readonly IChangeFeedContainerProcessorProvider _changeFeedContainerProcessorProvider;
        private IEnumerable<IChangeFeedContainerProcessor> _processors;

        public DefaultChangeFeedService(IChangeFeedContainerProcessorProvider changeFeedContainerProcessorProvider)
        {
            _processors = new List<IChangeFeedContainerProcessor>();
            _changeFeedContainerProcessorProvider = changeFeedContainerProcessorProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _processors = _changeFeedContainerProcessorProvider.GetProcessors();
            return Task.WhenAll(_processors.Select(x => x.StartAsync()));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (IChangeFeedContainerProcessor processor in _processors)
            {
                if (processor != null)
                {
                    await processor.StopAsync();
                }
            }
        }
    }
}