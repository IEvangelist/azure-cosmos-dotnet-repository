// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc/>
    internal class DefaultCosmosClientProvider : ICosmosClientProvider, IDisposable
    {
        readonly Lazy<CosmosClient> _lazyCosmosClient;
        readonly CosmosClientOptions _cosmosClientOptions;
        readonly RepositoryOptions _options;

        /// <inheritdoc/>
        public DefaultCosmosClientProvider(
            CosmosClientOptions cosmosClientOptions,
            IOptions<RepositoryOptions> options)
        {
            _cosmosClientOptions = cosmosClientOptions
                ?? throw new ArgumentNullException(
                    nameof(cosmosClientOptions), "Cosmos Client options are required.");

            _options = options?.Value
                ?? throw new ArgumentNullException(
                    nameof(options), "Repository options are required.");

            _lazyCosmosClient = new Lazy<CosmosClient>(
                () => new CosmosClient(_options.CosmosConnectionString, _cosmosClientOptions));
        }

        /// <inheritdoc/>
        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) =>
            consume(_lazyCosmosClient.Value);

        /// <inheritdoc/>
        public void Dispose() => _lazyCosmosClient.Value?.Dispose();
    }
}
