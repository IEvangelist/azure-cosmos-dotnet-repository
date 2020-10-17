// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.CosmosRepository.Options;
    using Microsoft.Extensions.Options;

    /// <inheritdoc />
    internal class DefaultCosmosClientProvider : ICosmosClientProvider, IDisposable
    {
        private readonly CosmosClientOptions cosmosClientOptions;
        private readonly Lazy<CosmosClient> lazyCosmosClient;
        private readonly RepositoryOptions options;
        private bool disposedValue;

        /// <inheritdoc />
        public DefaultCosmosClientProvider(
            CosmosClientOptions cosmosClientOptions,
            IOptions<RepositoryOptions> options)
        {
            this.cosmosClientOptions = cosmosClientOptions
                ?? throw new ArgumentNullException(
                    nameof(cosmosClientOptions),
                    Properties.Resources.CosmosClientOptionsAreRequired);

            this.options = options?.Value
                ?? throw new ArgumentNullException(
                    nameof(options),
                    Properties.Resources.RepositoryOptionsAreRequired);

            this.lazyCosmosClient = new Lazy<CosmosClient>(
                () => new CosmosClient(this.options.CosmosConnectionString, this.cosmosClientOptions));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DefaultCosmosClientProvider" /> class.
        /// </summary>
        ~DefaultCosmosClientProvider()
        {
            this.Dispose(disposing: false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) => consume(this.lazyCosmosClient.Value);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue)
            {
                return;
            }

            if (disposing)
            {
                if (this.lazyCosmosClient.IsValueCreated)
                {
                    this.lazyCosmosClient.Value?.Dispose();
                }
            }

            this.disposedValue = true;
        }
    }
}
