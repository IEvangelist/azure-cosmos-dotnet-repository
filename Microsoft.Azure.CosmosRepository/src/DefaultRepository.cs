// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    internal class DefaultRepository<TItem> : IRepository<TItem> where TItem : ItemBase
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
        public async ValueTask<TItem> GetAsync(string id)
        {
            return await GetAsync(id, id);
        }

        /// <inheritdoc/>
        public async ValueTask<TItem> GetAsync(string id, string partitionKey)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<TItem> response = await container.ReadItemAsync<TItem>(id, new PartitionKey(partitionKey));

            TItem item = response.Resource;

            TryLogDebugDetails(_logger, () => $"Read: {JsonConvert.SerializeObject(item)}");

            return string.IsNullOrEmpty(item.Type) || item.Type == typeof(TItem).Name ? item : null;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate)
        {
            Container container = await _containerProvider.GetContainerAsync();

            IQueryable<TItem> query =
                container.GetItemLinqQueryable<TItem>()
                    .Where(predicate.Compose(
                        item => !item.Type.IsDefined() || item.Type == typeof(TItem).Name, Expression.AndAlso));

            TryLogDebugDetails(_logger, () => $"Read: {query}");

            using (FeedIterator<TItem> iterator = query.ToFeedIterator())
            {
                List<TItem> results = new List<TItem>();
                while (iterator.HasMoreResults)
                {
                    foreach (TItem result in await iterator.ReadNextAsync())
                    {
                        results.Add(result);
                    }
                }

                return results;
            }
        }

        /// <inheritdoc/>
        public async ValueTask<TItem> CreateAsync(TItem value)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<TItem> response =
                await container.CreateItemAsync(value, value.PartitionKey);

            TryLogDebugDetails(_logger, () => $"Created: {JsonConvert.SerializeObject(value)}");

            return response.Resource;
        }

        /// <inheritdoc/>
        public Task<TItem[]> CreateAsync(IEnumerable<TItem> values) =>
            Task.WhenAll(values.Select(v => CreateAsync(v).AsTask()));

        /// <inheritdoc/>
        public async ValueTask<TItem> UpdateAsync(TItem value)
        {
            (bool optimizeBandwidth, ItemRequestOptions options) = RequestOptions;
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<TItem> response =
                await container.UpsertItemAsync<TItem>(value, value.PartitionKey, options);

            TryLogDebugDetails(_logger, () => $"Updated: {JsonConvert.SerializeObject(value)}");

            return optimizeBandwidth ? value : response.Resource;
        }

        /// <inheritdoc/>
        public ValueTask DeleteAsync(TItem value) => DeleteAsync(value.Id);

        /// <inheritdoc/>
        public async ValueTask DeleteAsync(string id)
        {
            await DeleteAsync(id, id);
        }

        /// <inheritdoc/>
        public async ValueTask DeleteAsync(string id, string partitionKey)
        {
            ItemRequestOptions options = RequestOptions.Options;
            Container container = await _containerProvider.GetContainerAsync();
            _ = await container.DeleteItemAsync<TItem>(id, new PartitionKey(partitionKey), options);

            TryLogDebugDetails(_logger, () => $"Deleted: {id}");
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