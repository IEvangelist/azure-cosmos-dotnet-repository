// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Containers;
using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;
using Microsoft.Azure.Cosmos;

namespace Aspire.Microsoft.Azure.CosmosRepository.Internals.Containers;

internal class DefaultItemContainerProvider(
    IContainerCache containerCache,
    IItemConfiguration itemConfiguration) : IItemContainerProvider
{
    public async Task<Container> GetContainerAsync<TItem>(
        CancellationToken cancellationToken = default) where TItem : IItem
    {
        ICosmosItemConfiguration<TItem> configuration = itemConfiguration.For<TItem>();

        Container container = await containerCache.GetContainerAsync(
            configuration.Container,
            cancellationToken);

        //TODO: this is where auto-creation can occur

        return container;
    }
}