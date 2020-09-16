// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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
        readonly Lazy<Container> _lazyContainer;
        readonly ILogger<DefaultCosmosContainerProvider> _logger;

        CosmosClient _client;

        internal DefaultCosmosContainerProvider(
            IOptions<RepositoryOptions> options,
            ILogger<DefaultCosmosContainerProvider> logger)
        {
            _options = options?.Value
                ?? throw new ArgumentNullException(nameof(options), "Repository options are required.");

            _logger = logger;
            _lazyContainer = new Lazy<Container>(() =>
            {
                try
                {
                    _client = new CosmosClient(_options.CosmosConnectionString);
                    Database database = _client.GetDatabase(_options.DatabaseId);

                    return database.GetContainer(_options.ContainerId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);

                    throw;
                }
            });
        }

        /// <inheritdoc/>
        public Container GetContainer() => _lazyContainer.Value;

        /// <inheritdoc/>
        public void Dispose() => _client?.Dispose();
    }
}
