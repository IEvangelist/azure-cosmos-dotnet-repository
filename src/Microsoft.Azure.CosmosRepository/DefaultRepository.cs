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
using Microsoft.Azure.CosmosRepository.Logging;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Processors;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Specification;
using Microsoft.Azure.CosmosRepository.Specification.Evaluator;
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
        private readonly ISpecificationEvaluator _specificationEvaluator;

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
            IRepositoryExpressionProvider repositoryExpressionProvider,
            ISpecificationEvaluator specificationEvaluator) =>
            (_optionsMonitor, _containerProvider, _logger, _cosmosQueryableProcessor, _repositoryExpressionProvider, _specificationEvaluator) =
            (optionsMonitor, containerProvider, logger, cosmosQueryableProcessor, repositoryExpressionProvider, specificationEvaluator);

        /// <inheritdoc/>
        public ValueTask<TItem> GetAsync(
            string id,
            string? partitionKeyValue = null,
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

            _logger.LogPointReadStarted<TItem>(id, partitionKey.ToString());

            ItemResponse<TItem> response =
                await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            TItem item = response.Resource;

            _logger.LogPointReadExecuted<TItem>(response.RequestCharge);
            _logger.LogItemRead(item);

            return _repositoryExpressionProvider.CheckItem(item)
                   ?? throw new NotImplementedException();
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
                    .Where(_repositoryExpressionProvider.Build(predicate)
                    ?? Enumerable.Empty<TItem>().AsQueryable();

            _logger.LogQueryConstructed(query);

            (IEnumerable<TItem> items, double charge) =
                await _cosmosQueryableProcessor.IterateAsync(query, cancellationToken);

            _logger.LogQueryExecuted(query, charge);

            return items;
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

            if (value is IItemWithTimeStamps {CreatedTimeUtc: null} valueWithTimestamps)
            {
                valueWithTimestamps.CreatedTimeUtc = DateTime.UtcNow;
            }

            ItemResponse<TItem> response =
                await container.CreateItemAsync(value, new PartitionKey(value.PartitionKey),
                        cancellationToken: cancellationToken)
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
            bool ignoreEtag = false)
        {
            (bool optimizeBandwidth, ItemRequestOptions options) = RequestOptions;
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            if (value is IItemWithEtag valueWithEtag && !ignoreEtag)
            {
                options.IfMatchEtag = string.IsNullOrWhiteSpace(valueWithEtag.Etag) ? default : valueWithEtag.Etag;
            }

            ItemResponse<TItem> response =
                await container.UpsertItemAsync(value, new PartitionKey(value.PartitionKey), options,
                        cancellationToken)
                    .ConfigureAwait(false);

            TryLogDebugDetails(_logger, () => $"Updated: {JsonConvert.SerializeObject(value)}");

            return optimizeBandwidth ? value : response.Resource;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> UpdateAsync(
            IEnumerable<TItem> values,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false)
        {
            IEnumerable<Task<TItem>> updateTasks =
                values.Select(value => UpdateAsync(value, cancellationToken, ignoreEtag).AsTask())
                    .ToList();

            await Task.WhenAll(updateTasks).ConfigureAwait(false);

            return updateTasks.Select(x => x.Result);
        }

        public async ValueTask UpdateAsync(string id,
            Action<IPatchOperationBuilder<TItem>> builder,
            string? partitionKeyValue = null,
            CancellationToken cancellationToken = default,
            string? etag = default)
        {
            IPatchOperationBuilder<TItem> patchOperationBuilder = new PatchOperationBuilder<TItem>();

            builder(patchOperationBuilder);

            Container container = await _containerProvider.GetContainerAsync();

            partitionKeyValue ??= id;

            PatchItemRequestOptions patchItemRequestOptions = new();
            if (etag != default && !string.IsNullOrWhiteSpace(etag))
            {
                patchItemRequestOptions.IfMatchEtag = etag;
            }

            await container.PatchItemAsync<TItem>(id, new PartitionKey(partitionKeyValue),
                patchOperationBuilder.PatchOperations, patchItemRequestOptions, cancellationToken);
        }

        /// <inheritdoc/>
        public ValueTask DeleteAsync(
            TItem value,
            CancellationToken cancellationToken = default) =>
            DeleteAsync(value.Id, value.PartitionKey, cancellationToken);

        /// <inheritdoc/>
        public ValueTask DeleteAsync(
            string id,
            string? partitionKeyValue = null,
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
        public ValueTask<bool> ExistsAsync(
            string id,
            string? partitionKeyValue = null,
            CancellationToken cancellationToken = default)
            => ExistsAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync(string id, PartitionKey partitionKey,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            try
            {
                _ = await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)

            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query =
                container.GetItemLinqQueryable<TItem>()
                    .Where(_repositoryExpressionProvider.Build(predicate) ?? throw new NotImplementedException());

            TryLogDebugDetails(_logger, () => $"Read: {query}");

            int count = await _cosmosQueryableProcessor.CountAsync(query, cancellationToken);
            return count > 0;
        }

        /// <inheritdoc/>
        public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>();

            TryLogDebugDetails(_logger, () => $"Read: {query}");

            return await _cosmosQueryableProcessor.CountAsync(query, cancellationToken);
        }

        private async ValueTask<Response<int>> CountAsync<TResult>(ISpecification<TItem, TResult> specification, CancellationToken cancellationToken = default)
            where TResult : IQueryResult<TItem>
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>();

            query = _specificationEvaluator.GetQuery(query, specification, evaluateCriteriaOnly: true);

            TryLogDebugDetails(_logger, () => $"Read: {query}");
            return await query.CountAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async ValueTask<int> CountAsync(
            Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Container container =
                await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query =
                container.GetItemLinqQueryable<TItem>()
                    .Where(_repositoryExpressionProvider.Build(predicate) ?? throw new NotImplementedException());

            TryLogDebugDetails(_logger, () => $"Read: {query}");

            return await _cosmosQueryableProcessor.CountAsync(query, cancellationToken);
        }

        /// <inheritdoc/>
        public async ValueTask<IPage<TItem>> PageAsync(
            Expression<Func<TItem, bool>>? predicate = null,
            int pageSize = 25,
            string? continuationToken = null,
            CancellationToken cancellationToken = default)
        {
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            QueryRequestOptions options = new()
            {
                MaxItemCount = pageSize
            };

            IQueryable<TItem> query = container
                .GetItemLinqQueryable<TItem>(requestOptions: options, continuationToken: continuationToken)
                .Where(_repositoryExpressionProvider.Build(predicate) ?? throw new NotImplementedException());

            _logger.LogQueryConstructed(query);

            Response<int> countResponse = await query.CountAsync(cancellationToken);

            (List<TItem> Items, double Charge, string? ContinuationToken) result =
                await GetAllItemsAsync(query, pageSize, cancellationToken);

            _logger.LogQueryExecuted(query, result.Charge);

            return new Page<TItem>(
                countResponse.Resource,
                pageSize,
                result.Items.AsReadOnly(),
                result.Charge + countResponse.RequestCharge,
                result.ContinuationToken);
        }

        /// <inheritdoc/>
        public async ValueTask<IPageQueryResult<TItem>> PageAsync(
            Expression<Func<TItem, bool>>? predicate = null,
            int pageNumber = 1,
            int pageSize = 25,
            CancellationToken cancellationToken = default)
        {
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            IQueryable<TItem> query = container
                .GetItemLinqQueryable<TItem>()
                .Where(_repositoryExpressionProvider.Build(predicate) ?? throw new NotImplementedException());

            Response<int> countResponse =
                await query.CountAsync(cancellationToken).ConfigureAwait(false);

            query = query.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);

            _logger.LogQueryConstructed(query);

            (List<TItem> Items, double Charge, string? ContinuationToken) result =
                await GetAllItemsAsync(query, pageSize, cancellationToken);

            _logger.LogQueryExecuted(query, result.Charge);

            return new PageQueryResult<TItem>(
                countResponse.Resource,
                pageNumber,
                pageSize,
                result.Items.AsReadOnly(),
                result.Charge + countResponse.RequestCharge);
        }

        /// <inheritdoc/>
        public async ValueTask<TResult> QueryAsync<TResult>(
            ISpecification<TItem, TResult> specification,
            CancellationToken cancellationToken = default)
            where TResult : IQueryResult<TItem>
        {
            Container container = await _containerProvider.GetContainerAsync().ConfigureAwait(false);

            QueryRequestOptions options = new();

            if (specification.UseContinuationToken)
            {
                options.MaxItemCount = specification.PageSize;
            }

            IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>(requestOptions: options,continuationToken: specification.ContinuationToken)
                .Where(_repositoryExpressionProvider.Build<TItem>(null) ?? throw new NotImplementedException());

            query = _specificationEvaluator.GetQuery(query, specification);

            _logger.LogQueryConstructed(query);

            (List<TItem> items, double charge, string? continuationToken) = await GetAllItemsAsync(query, specification.PageSize, cancellationToken).ConfigureAwait(false);

            _logger.LogQueryExecuted(query, charge);

            Response<int> count = await CountAsync(specification, cancellationToken).ConfigureAwait(false);

            return specification.PostProcessingAction(items.AsReadOnly(), count.Resource, charge + count.RequestCharge, continuationToken);
        }

        static void TryLogDebugDetails(ILogger logger, Func<string> getMessage)
        {
            if (logger?.IsEnabled(LogLevel.Debug) ?? false)
            {
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                logger.LogDebug(getMessage());
            }
        }

        static async Task<(List<TItem> items, double charge, string? continuationToken)> GetAllItemsAsync(
            IQueryable<TItem> query,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            string? continuationToken = null;
            List<TItem> results = new();
            int readItemsCount = 0;
            double charge = 0;
            using FeedIterator<TItem> iterator = query.ToFeedIterator();
            while (readItemsCount < pageSize && iterator.HasMoreResults)
            {
                FeedResponse<TItem> next = await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);

                foreach (TItem result in next)
                {
                    if (readItemsCount == pageSize)
                    {
                        break;
                    }

                    results.Add(result);
                    readItemsCount++;
                }

                charge += next.RequestCharge;
                continuationToken = next.ContinuationToken;
            }

            return (results, charge, continuationToken);
        }

    }
}