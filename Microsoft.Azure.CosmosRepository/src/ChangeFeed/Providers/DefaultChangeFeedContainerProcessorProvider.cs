// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Processors;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers
{
    internal class DefaultChangeFeedContainerProcessorProvider : IChangeFeedContainerProcessorProvider
    {
        private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
        private readonly ICosmosContainerService _containerService;
        private readonly ILeaseContainerProvider _leaseContainerProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceProvider _serviceProvider;

        public DefaultChangeFeedContainerProcessorProvider(
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICosmosContainerService containerService,
            ILeaseContainerProvider leaseContainerProvider,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            _optionsMonitor = optionsMonitor;
            _containerService = containerService;
            _leaseContainerProvider = leaseContainerProvider;
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
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
                yield return new DefaultChangeFeedContainerProcessor(
                    _containerService,
                    container.Select(x => x.Type).ToList(),
                    _leaseContainerProvider,
                    _loggerFactory.CreateLogger<DefaultChangeFeedContainerProcessor>(),
                    _serviceProvider);
            }
        }
    }
}