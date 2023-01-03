// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Services;

/// <inheritdoc/>
class DefaultCosmosContainerSyncService : ICosmosContainerSyncService
{
    readonly ICosmosContainerService _containerService;

    public DefaultCosmosContainerSyncService(ICosmosContainerService containerService) =>
        _containerService = containerService;

    public Task SyncContainerPropertiesAsync<TItem>() where TItem : IItem =>
        _containerService.GetContainerAsync<TItem>(true);
}