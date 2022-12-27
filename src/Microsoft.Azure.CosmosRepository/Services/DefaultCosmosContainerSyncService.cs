// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.Services;

/// <inheritdoc/>
class DefaultCosmosContainerSyncService : ICosmosContainerSyncService
{
    readonly ICosmosContainerService _containerService;

    public DefaultCosmosContainerSyncService(ICosmosContainerService containerService) => _containerService = containerService;

    public Task SyncContainerPropertiesAsync<TItem>() where TItem : IItem => _containerService.GetContainerAsync<TItem>(true);
}