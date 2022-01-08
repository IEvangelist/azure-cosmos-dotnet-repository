// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Services;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Processors
{
    internal class DefaultChangeFeedContainerProcessor : IChangeFeedContainerProcessor
    {
        private readonly ICosmosClientProvider _clientProvider;
        private readonly ICosmosContainerService _containerService;

        public DefaultChangeFeedContainerProcessor(ICosmosClientProvider clientProvider, ICosmosContainerService containerService)
        {
            _clientProvider = clientProvider;
            _containerService = containerService;
        }

        public Task StartAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}