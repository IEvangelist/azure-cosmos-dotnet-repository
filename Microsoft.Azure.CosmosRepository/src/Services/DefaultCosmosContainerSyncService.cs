// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.Services
{
    /// <inheritdoc/>
    class DefaultCosmosContainerSyncService : ICosmosContainerSyncService
    {
        readonly ICosmosContainerService _containerService;

        public DefaultCosmosContainerSyncService(ICosmosContainerService containerService) => _containerService = containerService;

        public Task SyncAsync<TItem>() where TItem : IItem => _containerService.GetContainerAsync<TItem>(true);
    }
}