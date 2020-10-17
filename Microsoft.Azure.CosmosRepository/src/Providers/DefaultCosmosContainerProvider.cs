// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.CosmosRepository.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <inheritdoc />
    internal class DefaultCosmosContainerProvider<TItem>
        : ICosmosContainerProvider<TItem> where TItem : Item
    {
        private readonly ICosmosClientProvider cosmosClientProvider;
        private readonly ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider;
        private readonly Lazy<Task<Container>> lazyContainer;
        private readonly ILogger<DefaultCosmosContainerProvider<TItem>> logger;
        private readonly RepositoryOptions options;

        public DefaultCosmosContainerProvider(
            ICosmosClientProvider cosmosClientProvider,
            ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider,
            IOptions<RepositoryOptions> options,
            ILogger<DefaultCosmosContainerProvider<TItem>> logger)
        {
            this.cosmosClientProvider = cosmosClientProvider
                ?? throw new ArgumentNullException(
                    nameof(cosmosClientProvider), Properties.Resources.CosmosClientProviderIsRequired);

            this.cosmosPartitionKeyPathProvider = cosmosPartitionKeyPathProvider
                ?? throw new ArgumentNullException(
                    nameof(cosmosPartitionKeyPathProvider), Properties.Resources.CosmosPartitionKeyNameProviderIsRequired);

            this.options = options?.Value
                ?? throw new ArgumentNullException(
                    nameof(options), Properties.Resources.RepositoryOptionsAreRequired);

            if (this.options.CosmosConnectionString is null)
            {
                throw new NullReferenceException(
                    string.Format(
                        Properties.Resources.TheInsertNameHereIsRequired,
                        nameof(DefaultCosmosContainerProvider<TItem>.options.CosmosConnectionString)));
            }

            if (this.options.ContainerPerItemType is false)
            {
                if (this.options.DatabaseId is null)
                {
                    throw new NullReferenceException(
                        string.Format(
                            Properties.Resources.TheInsertNameHereIsRequiredWhenContainerPerItemTypeIsFalse,
                            nameof(DefaultCosmosContainerProvider<TItem>.options.DatabaseId)));
                }

                if (this.options.ContainerId is null)
                {
                    throw new NullReferenceException(
                        string.Format(
                            Properties.Resources.TheInsertNameHereIsRequiredWhenContainerPerItemTypeIsFalse,
                            nameof(DefaultCosmosContainerProvider<TItem>.options.ContainerId)));
                }
            }

            this.logger = logger ?? throw new ArgumentNullException(
                string.Format(Properties.Resources.TheInsertNameHereIsRequired, nameof(logger)));

            this.lazyContainer = new Lazy<Task<Container>>(async () =>
            {
                try
                {
                    Database database =
                        await this.cosmosClientProvider.UseClientAsync(
                            client => client.CreateDatabaseIfNotExistsAsync(this.options.DatabaseId)).ConfigureAwait(false);

                    ContainerProperties containerProperties = new ContainerProperties
                    {
                        Id = this.options.ContainerPerItemType ? typeof(TItem).Name : this.options.ContainerId,
                        PartitionKeyPath = this.cosmosPartitionKeyPathProvider.GetPartitionKeyPath<TItem>()
                    };

                    Container container =
                        await database.CreateContainerIfNotExistsAsync(containerProperties).ConfigureAwait(false);

                    return container;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex.Message, ex);
                    throw;
                }
            });
        }

        /// <inheritdoc />
        public Task<Container> GetContainerAsync() => this.lazyContainer.Value;
    }
}
