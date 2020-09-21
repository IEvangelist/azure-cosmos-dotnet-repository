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
    internal class DefaultRepository<T> : IRepository<T> where T : Document
    {
        readonly ICosmosContainerProvider _containerProvider;
        readonly ILogger<T> _logger;

        public DefaultRepository(
            ICosmosContainerProvider containerProvider,
            ILogger<T> logger) =>
            (_containerProvider, _logger) = (containerProvider, logger);

        /// <inheritdoc/>
        public async ValueTask<T> GetAsync(string id)
        {
            try
            {
                Container container = await _containerProvider.GetContainerAsync();
                ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message, ex);
                return default;
            }
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                Container container = await _containerProvider.GetContainerAsync();
                using (FeedIterator<T> iterator =
                    container.GetItemLinqQueryable<T>()
                             .Where(predicate)
                             .ToFeedIterator())
                {
                    List<T> results = new List<T>();
                    while (iterator.HasMoreResults)
                    {
                        foreach (T result in await iterator.ReadNextAsync())
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
                return Enumerable.Empty<T>();
            }
        }

        /// <inheritdoc/>
        public async ValueTask<T> CreateAsync(T value)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<T> response = await container.CreateItemAsync(value, value.PartitionKey);

            return response.Resource;
        }

        /// <inheritdoc/>
        public Task<T[]> CreateAsync(IEnumerable<T> values) =>
            Task.WhenAll(values.Select(v => CreateAsync(v).AsTask()));

        /// <inheritdoc/>
        public async ValueTask<T> UpdateAsync(T value)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<T> response = await container.UpsertItemAsync<T>(value, value.PartitionKey);

            return response.Resource;
        }

        /// <inheritdoc/>
        public ValueTask<T> DeleteAsync(T value) => DeleteAsync(value.Id);

        /// <inheritdoc/>
        public async ValueTask<T> DeleteAsync(string id)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<T> response = await container.DeleteItemAsync<T>(id, new PartitionKey(id));

            return response.Resource;
        }
    }
}