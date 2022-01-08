// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Processors;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers
{
    internal class DefaultChangeFeedContainerProcessorProvider : IChangeFeedContainerProcessorProvider
    {
        private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
        private readonly ICosmosClientProvider _cosmosClientProvider;
        private readonly ICosmosContainerService _containerService;

        public DefaultChangeFeedContainerProcessorProvider(IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICosmosClientProvider cosmosClientProvider, ICosmosContainerService containerService)
        {
            _optionsMonitor = optionsMonitor;
            _cosmosClientProvider = cosmosClientProvider;
            _containerService = containerService;
        }

        public IEnumerable<IChangeFeedContainerProcessor> GetProcessors()
        {
            RepositoryOptions repositoryOptions = _optionsMonitor.CurrentValue;

            IEnumerable<IGrouping<string, ContainerOptionsBuilder>> containers = repositoryOptions.ContainerBuilder
                .Options
                .Where(x => x.ChangeFeedOptions != null)
                .GroupBy(x => x.Name);


            foreach (IGrouping<string, ContainerOptionsBuilder> container in containers)
            {
                yield return new DefaultChangeFeedContainerProcessor(_cosmosClientProvider,
                    _containerService,
                    container.Select(x => x.Type).ToList());
            }
        }
    }
}