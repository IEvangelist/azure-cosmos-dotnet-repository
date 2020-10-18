// Copyright © IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Properties;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository
{
	/// <inheritdoc />
	internal class DefaultRepository<TItem> : IRepository<TItem> where TItem : Item
	{
		private readonly ICosmosContainerProvider<TItem> _containerProvider;
		private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
		private readonly ILogger<DefaultRepository<TItem>> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRepository{TItem}" /> class.
		/// </summary>
		/// <param name="optionsMonitor">The options monitor.</param>
		/// <param name="containerProvider">The container provider.</param>
		/// <param name="logger">The logger.</param>
		public DefaultRepository(
			IOptionsMonitor<RepositoryOptions> optionsMonitor,
			ICosmosContainerProvider<TItem> containerProvider,
			ILogger<DefaultRepository<TItem>> logger) =>
			(
				_optionsMonitor,
				_containerProvider,
				_logger
			) = (
				optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor)),
				containerProvider ?? throw new ArgumentNullException(nameof(containerProvider)),
				logger ?? throw new ArgumentNullException(nameof(logger))
			);

		private (bool OptimizeBandwidth, ItemRequestOptions Options) RequestOptions =>
			(
				_optionsMonitor.CurrentValue.OptimizeBandwidth,
				new ItemRequestOptions
				{
					EnableContentResponseOnWrite = !_optionsMonitor.CurrentValue.OptimizeBandwidth
				}
			);

		/// <inheritdoc />
		public ValueTask<TItem> GetAsync(
			string id,
			string partitionKeyValue,
			CancellationToken cancellationToken = default) =>
			GetAsync(
				id ?? throw new ArgumentNullException(nameof(id)),
				new PartitionKey(partitionKeyValue ?? id),
				cancellationToken);

		/// <inheritdoc />
		public async ValueTask<TItem> GetAsync(
			string id,
			PartitionKey partitionKey = default,
			CancellationToken cancellationToken = default)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));

			if (partitionKey == default)
			{
				partitionKey = new PartitionKey(id);
			}

			Container container =
				await _containerProvider.GetContainerAsync().ConfigureAwait(false);

			ItemResponse<TItem> response = await container.ReadItemAsync<TItem>(
				id, partitionKey, RequestOptions.Options, cancellationToken).ConfigureAwait(false);

			TItem item = response.Resource;

			TryLogDebugDetails(_logger, () => Resources.ReadBlank.Format(JsonConvert.SerializeObject(item)));

			return string.IsNullOrEmpty(item.Type) || item.Type == typeof(TItem).Name ? item : null;
		}

		/// <inheritdoc />
		public async ValueTask<IEnumerable<TItem>> GetAsync(
			Expression<Func<TItem, bool>> predicate = null,
			CancellationToken cancellationToken = default)
		{
			predicate = predicate ?? (item => true);

			Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

			IQueryable<TItem> query =
				container.GetItemLinqQueryable<TItem>()
					.Where(
						predicate.Compose(
							item => !item.Type.IsDefined() || item.Type == typeof(TItem).Name,
							Expression.AndAlso));

			TryLogDebugDetails(_logger, () => Resources.ReadBlank.Format(query));

			using (FeedIterator<TItem> iterator = query.ToFeedIterator())
			{
				List<TItem> results = new List<TItem>();
				while (iterator.HasMoreResults)
				{
					foreach (TItem result in await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false))
					{
						results.Add(result);

						if (cancellationToken.IsCancellationRequested)
						{
							break;
						}
					}

					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}
				}

				return results;
			}
		}

		/// <inheritdoc />
		public async ValueTask<TItem> CreateAsync(TItem value, CancellationToken cancellationToken = default)
		{
			_ = value ?? throw new ArgumentNullException(nameof(value));

			Container container =
				await _containerProvider.GetContainerAsync().ConfigureAwait(false);

			ItemResponse<TItem> response = await container.CreateItemAsync(
				value, value.PartitionKey, RequestOptions.Options, cancellationToken).ConfigureAwait(false);

			TryLogDebugDetails(_logger, () => Resources.CreatedBlank.Format(JsonConvert.SerializeObject(value)));

			return response.Resource;
		}

		/// <inheritdoc />
		public async ValueTask<IEnumerable<TItem>> CreateAsync(
			IEnumerable<TItem> values,
			CancellationToken cancellationToken = default)
		{
			_ = values ?? throw new ArgumentNullException(nameof(values));

			IEnumerable<Task<TItem>> creationTasks =
				values.Select(v => CreateAsync(v, cancellationToken).AsTask()).ToList();

			_ = await Task.WhenAll(creationTasks).ConfigureAwait(false);

			return creationTasks.Select(x => x.Result);
		}

		/// <inheritdoc />
		public async ValueTask<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default)
		{
			_ = value ?? throw new ArgumentNullException(nameof(value));

			(bool optimizeBandwidth, ItemRequestOptions options) = RequestOptions;
			Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

			ItemResponse<TItem> response =
				await container.UpsertItemAsync(value, value.PartitionKey, options, cancellationToken).ConfigureAwait(false);

			TryLogDebugDetails(_logger, () => Resources.UpdatedBlank.Format(JsonConvert.SerializeObject(value)));

			return optimizeBandwidth ? value : response.Resource;
		}

		/// <inheritdoc />
		public ValueTask DeleteAsync(TItem value, CancellationToken cancellationToken = default)
		{
			_ = value ?? throw new ArgumentNullException(nameof(value));
			_ = value.Id ?? throw new ArgumentNullException(
				nameof(value),
				Resources.TheBlankIsRequired.Format(nameof(value.Id)));

			return DeleteAsync(value.Id, value.PartitionKey, cancellationToken);
		}

		/// <inheritdoc />
		public ValueTask DeleteAsync(
			string id,
			string partitionKeyValue,
			CancellationToken cancellationToken = default) =>
			DeleteAsync(
				id ?? throw new ArgumentNullException(nameof(id)),
				new PartitionKey(partitionKeyValue ?? id),
				cancellationToken);

		/// <inheritdoc />
		public async ValueTask DeleteAsync(
			string id,
			PartitionKey partitionKey = default,
			CancellationToken cancellationToken = default)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));

			if (partitionKey == default)
			{
				partitionKey = new PartitionKey(id);
			}

			ItemRequestOptions options = RequestOptions.Options;
			Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

			_ = await container.DeleteItemAsync<TItem>(id, partitionKey, options, cancellationToken).ConfigureAwait(false);

			TryLogDebugDetails(_logger, () => Resources.DeletedBlank.Format(id));
		}

		private static void TryLogDebugDetails(ILogger logger, Func<string> getMessage)
		{
			if (logger?.IsEnabled(LogLevel.Debug) ?? false)
			{
				logger.LogDebug(getMessage?.Invoke());
			}
		}
	}
}
