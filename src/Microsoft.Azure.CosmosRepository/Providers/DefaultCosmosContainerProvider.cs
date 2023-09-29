// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc/>
class DefaultCosmosContainerProvider<TItem>(ICosmosContainerService containerService)
    : ICosmosContainerProvider<TItem> where TItem : IItem
{
    readonly Lazy<Task<Container>> _lazyContainer = new(async () => await containerService.GetContainerAsync<TItem>());

    /// <inheritdoc/>
    public Task<Container> GetContainerAsync() => _lazyContainer.Value;
}
