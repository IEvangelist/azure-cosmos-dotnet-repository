// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosRepository
{
    /// <inheritdoc/>
    internal class DefaultRepository<TItem> : IRepository<TItem> where TItem : IItem
    {
        readonly ICosmosContainerProvider<TItem> _containerProvider;
        readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
        readonly ILogger<DefaultRepository<TItem>> _logger;

        (bool OptimizeBandwidth, ItemRequestOptions Options) RequestOptions =>
            (_optionsMonitor.CurrentValue.OptimizeBandwidth, new ItemRequestOptions
            {
                EnableContentResponseOnWrite = !_optionsMonitor.CurrentValue.OptimizeBandwidth
            });

        public DefaultRepository(
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICosmosContainerProvider<TItem> containerProvider,
            ILogger<DefaultRepository<TItem>> logger) =>
            (_optionsMonitor, _containerProvider, _logger) = (optionsMonitor, containerProvider, logger);

        /// <inheritdoc/>
        public ValueTask<TItem> GetAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default) =>
            GetAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<TItem> GetAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            ItemResponse<TItem> response =
                await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            TItem item = response.Resource;

            TryLogDebugDetails(_logger, () => $"Read: {JsonConvert.SerializeObject(item)}");

            return item is { Type: { Length: 0 } } || item.Type == typeof(TItem).Name ? item : default;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetAsync(
            Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query =
                container.GetItemLinqQueryable<TItem>()
                    .Where(predicate.Compose(
                        item => !item.Type.IsDefined() || item.Type == typeof(TItem).Name, Expression.AndAlso));

            TryLogDebugDetails(_logger, () => $"Read: {query}");

            using FeedIterator<TItem> iterator = query.ToFeedIterator();

            List<TItem> results = new();
            while (iterator.HasMoreResults)
            {
                foreach (TItem result in await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    results.Add(result);
                }
            }

            return results;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetByQueryAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Read {query}");

            QueryDefinition queryDefinition = new(query);
            return await IterateQueryInternalAsync(container, queryDefinition, cancellationToken);
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetByQueryAsync(
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Read {queryDefinition.QueryText}");

            return await IterateQueryInternalAsync(container, queryDefinition, cancellationToken);
        }

        /// <inheritdoc/>
        public async ValueTask<TItem> CreateAsync(
            TItem value,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            ItemResponse<TItem> response =
                await container.CreateItemAsync(value, value.PartitionKey, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Created: {JsonConvert.SerializeObject(value)}");

            return response.Resource;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> CreateAsync(
            IEnumerable<TItem> values,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Task<TItem>> creationTasks =
                values.Select(value => CreateAsync(value, cancellationToken).AsTask())
                    .ToList();

            _ = await Task.WhenAll(creationTasks).ConfigureAwait(false);

            return creationTasks.Select(x => x.Result);
        }

        /// <inheritdoc/>
        public async ValueTask<TItem> UpdateAsync(
            TItem value,
            CancellationToken cancellationToken = default)
        {
            (bool optimizeBandwidth, ItemRequestOptions options) = RequestOptions;
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            ItemResponse<TItem> response =
                await container.UpsertItemAsync<TItem>(value, value.PartitionKey, options, cancellationToken)
                    .ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Updated: {JsonConvert.SerializeObject(value)}");

            return optimizeBandwidth ? value : response.Resource;
        }

        /// <inheritdoc/>
        public ValueTask DeleteAsync(
            TItem value,
            CancellationToken cancellationToken = default) =>
            DeleteAsync(value.Id, value.PartitionKey, cancellationToken);

        /// <inheritdoc/>
        public ValueTask DeleteAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default) =>
            DeleteAsync(id, new PartitionKey(partitionKeyValue ?? id));

        /// <inheritdoc/>
        public async ValueTask DeleteAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default)
        {
            ItemRequestOptions options = RequestOptions.Options;
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            _ = await container.DeleteItemAsync<TItem>(id, partitionKey, options, cancellationToken)
                .ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Deleted: {id}");
        }

        static async Task<IEnumerable<TItem>> IterateQueryInternalAsync(
            Container container,
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken)
        {
            using FeedIterator<TItem> queryIterator = container.GetItemQueryIterator<TItem>(queryDefinition);

            List<TItem> results = new();
            while (queryIterator.HasMoreResults)
            {
                FeedResponse<TItem> response = await queryIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
                results.AddRange(response.Resource);
            }

            return results;
        }


        static void TryLogDebugDetails(ILogger logger, Func<string> getMessage)
        {
            if (logger?.IsEnabled(LogLevel.Debug) ?? false)
            {
                logger.LogDebug(getMessage());
            }
        }
    }
}