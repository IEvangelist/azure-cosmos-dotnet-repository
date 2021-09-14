// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Services
{
    class DefaultCosmosContainerService : ICosmosContainerService
    {
        readonly ICosmosItemConfigurationProvider _cosmosItemConfigurationProvider;
        readonly ICosmosClientProvider _cosmosClientProvider;
        readonly ILogger<DefaultCosmosContainerService> _logger;
        readonly RepositoryOptions _options;

        public DefaultCosmosContainerService(ICosmosItemConfigurationProvider cosmosItemConfigurationProvider,
            ICosmosClientProvider cosmosClientProvider,
            IOptions<RepositoryOptions> options,
            ILogger<DefaultCosmosContainerService> logger,
            IRepositoryOptionsValidator repositoryOptionsValidator)
        {
            repositoryOptionsValidator.ValidateForContainerCreation(options);
            _cosmosItemConfigurationProvider = cosmosItemConfigurationProvider;
            _cosmosClientProvider = cosmosClientProvider;
            _logger = logger;
            _options = options?.Value;
        }
        public async Task<Container> GetContainerAsync<TItem>(bool forceContainerSync = false) where TItem : IItem
        {
            try
            {
                ItemOptions itemOptions = _cosmosItemConfigurationProvider.GetOptions<TItem>();

                Database database =
                    await _cosmosClientProvider.UseClientAsync(
                        client => client.CreateDatabaseIfNotExistsAsync(_options.DatabaseId)).ConfigureAwait(false);

                ContainerProperties containerProperties = new()
                {
                    Id = _options.ContainerPerItemType
                        ? itemOptions.ContainerName
                        : _options.ContainerId,
                    PartitionKeyPath = itemOptions.PartitionKeyPath,
                    UniqueKeyPolicy = itemOptions.UniqueKeyPolicy ?? new(),
                    DefaultTimeToLive = itemOptions.DefaultTimeToLive
                };

                Container container =
                    await database.CreateContainerIfNotExistsAsync(
                        containerProperties).ConfigureAwait(false);

                if (itemOptions.SyncContainerProperties || forceContainerSync)
                {
                    await container.ReplaceContainerAsync(containerProperties);
                }

                return container;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                throw;
            }
        }
    }
}