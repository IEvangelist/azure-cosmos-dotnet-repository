// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
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
        readonly ICosmosPartitionKeyPathProvider _cosmosPartitionKeyPathProvider;
        readonly ICosmosContainerNameProvider _cosmosContainerNameProvider;
        readonly ICosmosUniqueKeyPolicyProvider _cosmosUniqueKeyPolicyProvider;
        readonly ILogger<DefaultCosmosContainerProvider<TItem>> _logger;

        public DefaultCosmosContainerProvider(
            ICosmosClientProvider cosmosClientProvider,
            ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider,
            ICosmosContainerNameProvider cosmosContainerNameProvider,
            ICosmosUniqueKeyPolicyProvider cosmosUniqueKeyPolicyProvider,
            IOptions<RepositoryOptions> options,
            ILogger<DefaultCosmosContainerProvider<TItem>> logger)
        {
            _cosmosClientProvider = cosmosClientProvider
                ?? throw new ArgumentNullException(
                    nameof(cosmosClientProvider), "Cosmos client provider is required.");

            _cosmosPartitionKeyPathProvider = cosmosPartitionKeyPathProvider
                ?? throw new ArgumentNullException(
                    nameof(cosmosPartitionKeyPathProvider), "Cosmos partition key name provider is required.");

            _cosmosContainerNameProvider = cosmosContainerNameProvider
                ?? throw new ArgumentNullException(
                    nameof(cosmosContainerNameProvider), "Cosmos container name provider is required.");

            _cosmosUniqueKeyPolicyProvider = cosmosUniqueKeyPolicyProvider
                ?? throw new ArgumentNullException(
                    nameof(cosmosContainerNameProvider), "Cosmos unique key policy provider is required.");

            _options = options?.Value
                ?? throw new ArgumentNullException(
                    nameof(options), "Repository options are required.");

            _logger = logger
                ?? throw new ArgumentNullException($"The {nameof(logger)} is required.");

            if (_options.CosmosConnectionString is null)
            {
                throw new ArgumentNullException($"The {nameof(_options.CosmosConnectionString)} is required.");
            }
            if (_options.ContainerPerItemType is false)
            {
                if (_options.DatabaseId is null)
                {
                    throw new ArgumentNullException(
                        $"The {nameof(_options.DatabaseId)} is required when container per item type is false.");
                }
                if (_options.ContainerId is null)
                {
                    throw new ArgumentNullException(
                        $"The {nameof(_options.ContainerId)} is required when container per item type is false.");
                }
            }

            _lazyContainer = new Lazy<Task<Container>>(async () => await GetContainerValueFactoryAsync());
            _cosmosContainerNameProvider = cosmosContainerNameProvider;
        }

        /// <inheritdoc/>
        public Task<Container> GetContainerAsync() => _lazyContainer.Value;

        async Task<Container> GetContainerValueFactoryAsync()
        {
            try
            {
                Database database =
                    await _cosmosClientProvider.UseClientAsync(
                        client => client.CreateDatabaseIfNotExistsAsync(_options.DatabaseId)).ConfigureAwait(false);

                ContainerProperties containerProperties = new()
                {
                    Id = _options.ContainerPerItemType
                        ? _cosmosContainerNameProvider.GetContainerName<TItem>()
                        : _options.ContainerId,
                    PartitionKeyPath = _cosmosPartitionKeyPathProvider.GetPartitionKeyPath<TItem>()
                };

                // Setting containerProperties.UniqueKeyPolicy to null throws, prevent that issue.
                UniqueKeyPolicy uniqueKeyPolicy = _cosmosUniqueKeyPolicyProvider.GetUniqueKeyPolicy<TItem>();
                if (uniqueKeyPolicy is not null)
                {
                    containerProperties.UniqueKeyPolicy = uniqueKeyPolicy;
                }

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
