// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc/>
    class DefaultCosmosContainerProvider<TItem>
        : ICosmosContainerProvider<TItem> where TItem : IItem
    {
        readonly Lazy<Task<Container>> _lazyContainer;
        readonly RepositoryOptions _options;
        readonly ICosmosClientProvider _cosmosClientProvider;
        private readonly ICosmosItemConfigurationProvider _cosmosItemConfigurationProvider;
        readonly ILogger<DefaultCosmosContainerProvider<TItem>> _logger;

        public DefaultCosmosContainerProvider(
            ICosmosClientProvider cosmosClientProvider,
            IOptions<RepositoryOptions> options,
            ICosmosItemConfigurationProvider cosmosItemConfigurationProvider,
            ILogger<DefaultCosmosContainerProvider<TItem>> logger,
            IRepositoryOptionsValidator repositoryOptionsValidator)
        {
            _cosmosClientProvider = cosmosClientProvider;
            _cosmosItemConfigurationProvider = cosmosItemConfigurationProvider;
            _options = options?.Value;
            _logger = logger;

            repositoryOptionsValidator.ValidateForContainerCreation(options);

            _lazyContainer = new Lazy<Task<Container>>(async () => await GetContainerValueFactoryAsync());
        }

        /// <inheritdoc/>
        public Task<Container> GetContainerAsync() => _lazyContainer.Value;

        async Task<Container> GetContainerValueFactoryAsync()
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
                    UniqueKeyPolicy = itemOptions.UniqueKeyPolicy ?? new()
                };

                Container container =
                    await database.CreateContainerIfNotExistsAsync(
                        containerProperties).ConfigureAwait(false);

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
