// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Services;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Processors
{
    internal class DefaultChangeFeedContainerProcessor : IChangeFeedContainerProcessor
    {
        private readonly ICosmosClientProvider _clientProvider;
        private readonly ICosmosContainerService _containerService;
        private readonly IReadOnlyList<Type> _itemTypes;

        public DefaultChangeFeedContainerProcessor(ICosmosClientProvider clientProvider,
            ICosmosContainerService containerService, IReadOnlyList<Type> itemTypes)
        {
            itemTypes.EnsureAllAreTypeOfIItem();
            _clientProvider = clientProvider;
            _containerService = containerService;
            _itemTypes = itemTypes;
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