// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Services;

/// <inheritdoc/>
class DefaultCosmosContainerSyncService(ICosmosContainerService containerService) : ICosmosContainerSyncService
{
    public Task SyncContainerPropertiesAsync<TItem>() where TItem : IItem =>
        containerService.GetContainerAsync<TItem>(true);
}