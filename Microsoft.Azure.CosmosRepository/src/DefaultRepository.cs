// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
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

    /// <inheritdoc />
    internal class DefaultRepository<TItem> : IRepository<TItem> where TItem : Item
    {
        private readonly ICosmosContainerProvider<TItem> containerProvider;
        private readonly ILogger<DefaultRepository<TItem>> logger;
        private readonly IOptionsMonitor<RepositoryOptions> optionsMonitor;

        public DefaultRepository(
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICosmosContainerProvider<TItem> containerProvider,
            ILogger<DefaultRepository<TItem>> logger) =>
            (this.optionsMonitor, this.containerProvider, this.logger) = (optionsMonitor, containerProvider, logger);

        private (bool OptimizeBandwidth, ItemRequestOptions Options) RequestOptions =>
            (this.optionsMonitor.CurrentValue.OptimizeBandwidth, new ItemRequestOptions
            {
                EnableContentResponseOnWrite = !this.optionsMonitor.CurrentValue.OptimizeBandwidth
            });

        /// <inheritdoc />
        public async ValueTask<TItem> CreateAsync(TItem value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            Container container =
                await this.containerProvider.GetContainerAsync().ConfigureAwait(false);

            ItemResponse<TItem> response =
                await container.CreateItemAsync(value, value.PartitionKey).ConfigureAwait(false);

            TryLogDebugDetails(this.logger, () => $"Created: {JsonConvert.SerializeObject(value)}");

            return response.Resource;
        }

        /// <inheritdoc />
        public async ValueTask<IEnumerable<TItem>> CreateAsync(IEnumerable<TItem> values)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            IEnumerable<Task<TItem>> creationTasks =
                values.Select(v => this.CreateAsync(v).AsTask()).ToList();

            TItem[]? results = await Task.WhenAll(creationTasks).ConfigureAwait(false);

            return results ?? Enumerable.Empty<TItem>();
        }

        /// <inheritdoc />
        public ValueTask DeleteAsync(TItem value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            return this.DeleteAsync(value.Id, value.PartitionKey);
        }

        /// <inheritdoc />
        public ValueTask DeleteAsync(string id, string? partitionKeyValue = null)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            return this.DeleteAsync(id, new PartitionKey(partitionKeyValue ?? id));
        }

        /// <inheritdoc />
        public async ValueTask DeleteAsync(string id, PartitionKey partitionKey)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));

            ItemRequestOptions options = this.RequestOptions.Options;
            Container container = await this.containerProvider.GetContainerAsync().ConfigureAwait(false);

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            _ = await container.DeleteItemAsync<TItem>(id, partitionKey, options).ConfigureAwait(false);

            TryLogDebugDetails(this.logger, () => $"Deleted: {id}");
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TItem> EnumerateAsync(
            Expression<Func<TItem, bool>>? predicate = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            predicate ??= item => true;

            Container container = await this.containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query =
                container.GetItemLinqQueryable<TItem>()
                    .Where(
                        predicate.Compose(
                            item => !item.Type.IsDefined() || item.Type == typeof(TItem).Name,
                            Expression.AndAlso));

            TryLogDebugDetails(
                this.logger,
                () => string.Format(
                    Properties.Resources.ReadInsertNameHere,
                    query));

            using FeedIterator<TItem> iterator = query.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (TItem result in await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    yield return result;
                }
            }
        }

        /// <inheritdoc />
        public ValueTask<TItem?> GetAsync(string id, string? partitionKeyValue = null)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));

            return this.GetAsync(id, new PartitionKey(partitionKeyValue ?? id));
        }

        /// <inheritdoc />
        public async ValueTask<TItem?> GetAsync(string id, PartitionKey partitionKey)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            Container container =
                await this.containerProvider.GetContainerAsync().ConfigureAwait(false);

            ItemResponse<TItem> response =
                await container.ReadItemAsync<TItem>(id, partitionKey).ConfigureAwait(false);

            TItem item = response.Resource;

            TryLogDebugDetails(
                this.logger,
                () => string.Format(
                    Properties.Resources.ReadInsertNameHere,
                    JsonConvert.SerializeObject(item)));

            return string.IsNullOrEmpty(item.Type) || item.Type == typeof(TItem).Name ? item : null;
        }

        /// <inheritdoc />
        public async ValueTask<IEnumerable<TItem>> GetAsync(
            Expression<Func<TItem, bool>>? predicate = null,
            CancellationToken cancellationToken = default) =>
            await this.EnumerateAsync(predicate, cancellationToken).ToListAsync();

        /// <inheritdoc />
        public async ValueTask<TItem> UpdateAsync(TItem value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            (bool optimizeBandwidth, ItemRequestOptions options) = this.RequestOptions;
            Container container = await this.containerProvider.GetContainerAsync().ConfigureAwait(false);

            ItemResponse<TItem> response =
                await container.UpsertItemAsync(value, value.PartitionKey, options).ConfigureAwait(false);

            TryLogDebugDetails(
                this.logger,
                () => string.Format(
                    Properties.Resources.UpdatedInsertNameHere,
                    JsonConvert.SerializeObject(value)));

            return optimizeBandwidth ? value : response.Resource;
        }

        private static void TryLogDebugDetails(ILogger? logger, Func<string> getMessage)
        {
            if (logger?.IsEnabled(LogLevel.Debug) ?? false)
            {
                logger.LogDebug(getMessage());
            }
        }
    }
}
