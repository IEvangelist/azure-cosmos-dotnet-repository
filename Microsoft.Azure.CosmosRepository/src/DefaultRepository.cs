// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository
{
    /// <inheritdoc/>
    internal class DefaultRepository<TItem> : IRepository<TItem> where TItem : Item
    {
        readonly ICosmosContainerProvider _containerProvider;
        readonly ILogger<DefaultRepository<TItem>> _logger;

        public DefaultRepository(
            ICosmosContainerProvider containerProvider,
            ILogger<DefaultRepository<TItem>> logger) =>
            (_containerProvider, _logger) = (containerProvider, logger);

        /// <inheritdoc/>
        public async ValueTask<TItem> GetAsync(string id)
        {
            try
            {
                Container container = await _containerProvider.GetContainerAsync();
                ItemResponse<TItem> response = await container.ReadItemAsync<TItem>(id, new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message, ex);
                return default;
            }
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate)
        {
            try
            {
                Container container = await _containerProvider.GetContainerAsync();
                using (FeedIterator<TItem> iterator =
                    container.GetItemLinqQueryable<TItem>()
                             .Where(predicate)
                             .ToFeedIterator())
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
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message, ex);
                return Enumerable.Empty<TItem>();
            }
        }

        /// <inheritdoc/>
        public async ValueTask<TItem> CreateAsync(TItem value)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<TItem> response = await container.CreateItemAsync(value, value.PartitionKey);

            return response.Resource;
        }

        /// <inheritdoc/>
        public Task<TItem[]> CreateAsync(IEnumerable<TItem> values) =>
            Task.WhenAll(values.Select(v => CreateAsync(v).AsTask()));

        /// <inheritdoc/>
        public async ValueTask<TItem> UpdateAsync(TItem value)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<TItem> response = await container.UpsertItemAsync<TItem>(value, value.PartitionKey);

            return response.Resource;
        }

        /// <inheritdoc/>
        public ValueTask<TItem> DeleteAsync(TItem value) => DeleteAsync(value.Id);

        /// <inheritdoc/>
        public async ValueTask<TItem> DeleteAsync(string id)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<TItem> response = await container.DeleteItemAsync<TItem>(id, new PartitionKey(id));

            return response.Resource;
        }
    }
}