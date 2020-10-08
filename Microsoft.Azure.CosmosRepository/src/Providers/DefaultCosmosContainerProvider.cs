﻿// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc/>
    internal class DefaultCosmosContainerProvider<TItem>
        : ICosmosContainerProvider<TItem> where TItem : ItemBase
    {
        readonly Lazy<Task<Container>> _lazyContainer;
        readonly RepositoryOptions _options;
        readonly ICosmosClientProvider _cosmosClientProvider;
        readonly ILogger<DefaultCosmosContainerProvider<TItem>> _logger;

        public DefaultCosmosContainerProvider(
            ICosmosClientProvider cosmosClientProvider,
            IOptions<RepositoryOptions> options,
            ILogger<DefaultCosmosContainerProvider<TItem>> logger)
        {
            _cosmosClientProvider = cosmosClientProvider
                ?? throw new ArgumentNullException(
                    nameof(cosmosClientProvider), "Cosmos client provider is required.");

            _options = options?.Value
                ?? throw new ArgumentNullException(
                    nameof(options), "Repository options are required.");

            if (_options.CosmosConnectionString is null)
            {
                throw new ArgumentNullException($"The {nameof(_options.CosmosConnectionString)} is required.");
            }
            if (_options.DatabaseId is null)
            {
                throw new ArgumentNullException($"The {nameof(_options.DatabaseId)} is required.");
            }
            if (_options.ContainerId is null)
            {
                throw new ArgumentNullException($"The {nameof(_options.ContainerId)} is required.");
            }

            _logger = logger ?? throw new ArgumentNullException($"The {nameof(logger)} is required.");

            _lazyContainer = new Lazy<Task<Container>>(async () =>
            {
                try
                {
                    Database database =
                        await _cosmosClientProvider.UseClientAsync(
                            client => client.CreateDatabaseIfNotExistsAsync(_options.DatabaseId));
                    Container container =
                        await database.CreateContainerIfNotExistsAsync(
                            new ContainerProperties
                            {
                                Id = _options.ContainerPerItemType ? typeof(TItem).Name : _options.ContainerId,
                                PartitionKeyPath = SetPartitionKeyPath(options.Value)
                            });

                    return container;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);

                    throw;
                }
            });
        }

        private static string SetPartitionKeyPath(RepositoryOptions repositoryOptions)
        {
            if (!repositoryOptions.ContainerPerItemType)
                return repositoryOptions.PartitionKeyPath;
            
            return typeof(TItem).GetCustomAttributes()
                       .OfType<PartitionKeyPathAttribute>()
                       .SingleOrDefault()
                       ?.PartitionKeyPath
                   ?? repositoryOptions.PartitionKeyPath;
        }

        /// <inheritdoc/>
        public Task<Container> GetContainerAsync() => _lazyContainer.Value;
    }
}
