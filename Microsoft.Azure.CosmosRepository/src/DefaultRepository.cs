// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Processors;
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
        readonly ICosmosQueryableProcessor _cosmosQueryableProcessor;
        readonly IRepositoryExpressionProvider _repositoryExpressionProvider;

        (bool OptimizeBandwidth, ItemRequestOptions Options) RequestOptions =>
            (_optionsMonitor.CurrentValue.OptimizeBandwidth, new ItemRequestOptions
            {
                EnableContentResponseOnWrite = !_optionsMonitor.CurrentValue.OptimizeBandwidth
            });

        public DefaultRepository(
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICosmosContainerProvider<TItem> containerProvider,
            ILogger<DefaultRepository<TItem>> logger,
            ICosmosQueryableProcessor cosmosQueryableProcessor,
            IRepositoryExpressionProvider repositoryExpressionProvider) =>
            (_optionsMonitor, _containerProvider, _logger, _cosmosQueryableProcessor, _repositoryExpressionProvider) =
                (optionsMonitor, containerProvider, logger, cosmosQueryableProcessor, repositoryExpressionProvider);

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
                    .Where(_repositoryExpressionProvider.Build(predicate));

            TryLogDebugDetails(_logger, () => $"Read: {query}");

            return await _cosmosQueryableProcessor.IterateAsync(query, cancellationToken);
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
            return await _cosmosQueryableProcessor.IterateAsync<TItem>(container, queryDefinition, cancellationToken);
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetByQueryAsync(
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Read {queryDefinition.QueryText}");

            return await _cosmosQueryableProcessor.IterateAsync<TItem>(container, queryDefinition, cancellationToken);
        }

        /// <inheritdoc/>
        public async ValueTask<TItem> CreateAsync(
            TItem value,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            ItemResponse<TItem> response =
                await container.CreateItemAsync(value, new PartitionKey(value.PartitionKey), cancellationToken: cancellationToken)
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
            CancellationToken cancellationToken = default,
            bool verifyEtag = false)
        {
            (bool optimizeBandwidth, ItemRequestOptions options) = RequestOptions;
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            ItemResponse<TItem> response =
                await container.UpsertItemAsync<TItem>(value, new PartitionKey(value.PartitionKey), options, cancellationToken)
                    .ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Updated: {JsonConvert.SerializeObject(value)}");

            return optimizeBandwidth ? value : response.Resource;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> UpdateAsync(
            IEnumerable<TItem> values,
            CancellationToken cancellationToken = default,
            bool verifyEtag = false)
        {
            IEnumerable<Task<TItem>> updateTasks =
                values.Select(value => UpdateAsync(value, cancellationToken, verifyEtag).AsTask())
                    .ToList();

            await Task.WhenAll(updateTasks).ConfigureAwait(false);

            return updateTasks.Select(x => x.Result);
        }

        public async ValueTask UpdateAsync(string id,
            Action<IPatchOperationBuilder<TItem>> builder,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default,
            string etag = default)
        {
            IPatchOperationBuilder<TItem> patchOperationBuilder = new PatchOperationBuilder<TItem>();

            builder(patchOperationBuilder);

            Container container = await _containerProvider.GetContainerAsync();

            partitionKeyValue ??= id;

            await container.PatchItemAsync<TItem>(id, new PartitionKey(partitionKeyValue),
                patchOperationBuilder.PatchOperations, cancellationToken: cancellationToken);
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
            DeleteAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

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

        /// <inheritdoc/>
        public ValueTask<bool> ExistsAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = default)
            => ExistsAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            try
            {
                ItemResponse<TItem> response =
                    await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)

            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query =
                container.GetItemLinqQueryable<TItem>()
                    .Where(_repositoryExpressionProvider.Build(predicate));

            TryLogDebugDetails(_logger, () => $"Read: {query}");

            int count = await _cosmosQueryableProcessor.CountAsync(query, cancellationToken);
            return count > 0;
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