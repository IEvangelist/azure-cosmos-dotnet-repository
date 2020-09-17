// Copyright (c) Microsoft Corporation. All rights reserved.
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
    internal class DefaultCosmosContainerProvider : ICosmosContainerProvider, IDisposable
    {
        readonly RepositoryOptions _options;
        readonly Lazy<Task<Container>> _lazyContainer;
        readonly ILogger<DefaultCosmosContainerProvider> _logger;

        CosmosClient _client;

        public DefaultCosmosContainerProvider(
            IOptions<RepositoryOptions> options,
            ILogger<DefaultCosmosContainerProvider> logger)
        {
            _options = options?.Value
                ?? throw new ArgumentNullException(nameof(options), "Repository options are required.");

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

            _logger = logger;
            _lazyContainer = new Lazy<Task<Container>>(async () =>
            {
                try
                {
                    _client = new CosmosClient(_options.CosmosConnectionString);
                    Database database = await _client.CreateDatabaseIfNotExistsAsync(_options.DatabaseId);
                    Container container = await database.CreateContainerIfNotExistsAsync(new ContainerProperties
                    {
                        Id = _options.ContainerId,
                        PartitionKeyPath = "/id"
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

        /// <inheritdoc/>
        public Task<Container> GetContainerAsync() => _lazyContainer.Value;

        /// <inheritdoc/>
        public void Dispose() => _client?.Dispose();
    }
}
