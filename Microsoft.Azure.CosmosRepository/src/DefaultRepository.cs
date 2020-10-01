// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository
{
    /// <inheritdoc/>
    internal class DefaultRepository<TItem> : IRepository<TItem> where TItem : Item
    {
        readonly ICosmosContainerProvider _containerProvider;
        readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;

        (bool optimizeBandwidth, ItemRequestOptions options) RequestOptions =>
            (_optionsMonitor.CurrentValue.OptimizeBandwidth, new ItemRequestOptions
            {
                EnableContentResponseOnWrite = !_optionsMonitor.CurrentValue.OptimizeBandwidth
            });

        public DefaultRepository(
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICosmosContainerProvider containerProvider) =>
            (_optionsMonitor, _containerProvider) = (optionsMonitor, containerProvider);

        /// <inheritdoc/>
        public async ValueTask<TItem> GetAsync(string id)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<TItem> response = await container.ReadItemAsync<TItem>(id, new PartitionKey(id));

            return response.Resource;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate)
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

        /// <inheritdoc/>
        public async ValueTask<TItem> CreateAsync(TItem value)
        {
            Container container = await _containerProvider.GetContainerAsync();
            ItemResponse<TItem> response =
                await container.CreateItemAsync(value, value.PartitionKey);

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

            return optimizeBandwidth ? value : response.Resource;
        }

        /// <inheritdoc/>
        public ValueTask DeleteAsync(TItem value) => DeleteAsync(value.Id);

        /// <inheritdoc/>
        public async ValueTask DeleteAsync(string id)
        {
            (_, ItemRequestOptions options) = RequestOptions;

            Container container = await _containerProvider.GetContainerAsync();
            _ = await container.DeleteItemAsync<TItem>(id, new PartitionKey(id), options);
        }
    }
}