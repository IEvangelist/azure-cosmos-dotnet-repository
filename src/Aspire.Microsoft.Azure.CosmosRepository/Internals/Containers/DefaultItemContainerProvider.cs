// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Containers;
using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Aspire.Microsoft.Azure.CosmosRepository.Internals.Containers;

internal class DefaultItemContainerProvider(
    ICosmosRepositoryClient cosmosRepositoryClient,
    IOptionsMonitor<AzureCosmosDbRepositorySettings> optionsMonitor,
    IItemConfiguration itemConfiguration) : IItemContainerProvider
{
    private readonly AzureCosmosDbRepositorySettings _settings = optionsMonitor.CurrentValue;

    public async Task<Container> GetContainerAsync<TItem>(
        CancellationToken cancellationToken = default) where TItem : IItem
    {
        ICosmosItemConfiguration<TItem> configuration = itemConfiguration.For<TItem>();

        Container container;

        if (_settings.IsAutomaticResourceCreationEnabled)
        {
            container = await cosmosRepositoryClient.CreateContainerAndDatabaseIfNotExistsAsync(
                _settings.DatabaseId,
                configuration.ContainerId,
                configuration.PartitionKeyPath);
        }
        else
        {
            container = cosmosRepositoryClient.GetContainer(
                _settings.DatabaseId,
                configuration.ContainerId);
        }

        return container;
    }
}