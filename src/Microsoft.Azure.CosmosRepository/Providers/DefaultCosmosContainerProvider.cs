// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Services;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc/>
    class DefaultCosmosContainerProvider<TItem>
        : ICosmosContainerProvider<TItem> where TItem : IItem
    {
        readonly Lazy<Task<Container>> _lazyContainer;

        public DefaultCosmosContainerProvider(ICosmosContainerService containerService) =>
            _lazyContainer = new Lazy<Task<Container>>(async () => await containerService.GetContainerAsync<TItem>());

        /// <inheritdoc/>
        public Task<Container> GetContainerAsync() => _lazyContainer.Value;
    }
}
