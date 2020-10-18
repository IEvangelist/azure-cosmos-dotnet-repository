// Copyright © IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.Providers
{
	/// <inheritdoc/>
	internal class DefaultCosmosContainerProvider<TItem>
		: ICosmosContainerProvider<TItem> where TItem : Item
	{
		readonly Lazy<Task<Container>> _lazyContainer;
		readonly RepositoryOptions _options;
		readonly ICosmosClientProvider _cosmosClientProvider;
		readonly ICosmosPartitionKeyPathProvider _cosmosPartitionKeyPathProvider;
		readonly ILogger<DefaultCosmosContainerProvider<TItem>> _logger;

		public DefaultCosmosContainerProvider(
			ICosmosClientProvider cosmosClientProvider,
			ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider,
			IOptions<RepositoryOptions> options,
			ILogger<DefaultCosmosContainerProvider<TItem>> logger)
		{
			_cosmosClientProvider = cosmosClientProvider ?? throw new ArgumentNullException(
				nameof(cosmosClientProvider), Resources.CosmosClientProviderIsRequired);

			_cosmosPartitionKeyPathProvider = cosmosPartitionKeyPathProvider ?? throw new ArgumentNullException(
				nameof(cosmosPartitionKeyPathProvider), Resources.CosmosPartitionKeyNameProviderIsRequired);

			_options = options?.Value ?? throw new ArgumentNullException(
				nameof(options), Resources.RepositoryOptionsAreRequired);

			_ = _options.CosmosConnectionString ?? throw new ArgumentNullException(
				nameof(options), Resources.TheBlankIsRequired.Format(nameof(_options.CosmosConnectionString)));

			if (_options.ContainerPerItemType is false)
			{
				_ = _options.DatabaseId ?? throw new ArgumentNullException(
					nameof(options),
					Resources.TheBlankIsRequiredWhenContainerPerItemTypeIsFalse.Format(nameof(_options.DatabaseId)));

				_ = _options.ContainerId ?? throw new ArgumentNullException(
					nameof(options),
					Resources.TheBlankIsRequiredWhenContainerPerItemTypeIsFalse.Format(nameof(_options.ContainerId)));
			}

			_logger = logger ?? throw new ArgumentNullException(
				nameof(logger), Resources.TheBlankIsRequired.Format(nameof(logger)));

			_lazyContainer = new Lazy<Task<Container>>(
				async () =>
				{
					try
					{
						Database database =
							await _cosmosClientProvider.UseClientAsync(
								client => client.CreateDatabaseIfNotExistsAsync(_options.DatabaseId)).ConfigureAwait(false);

						ContainerProperties containerProperties = new ContainerProperties
						{
							Id = _options.ContainerPerItemType ? typeof(TItem).Name : _options.ContainerId,
							PartitionKeyPath = _cosmosPartitionKeyPathProvider.GetPartitionKeyPath<TItem>()
						};

						Container container =
							await database.CreateContainerIfNotExistsAsync(containerProperties).ConfigureAwait(false);

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
	}
}
