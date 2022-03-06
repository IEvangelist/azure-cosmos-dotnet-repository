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
using Microsoft.Azure.CosmosRepository.Logging;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosRepository
{
    internal sealed partial class DefaultRepository<TItem>
    {
        /// <inheritdoc/>
        public async ValueTask<IPage<TItem>> PageAsync(
            Expression<Func<TItem, bool>>? predicate = null,
            int pageSize = 25,
            string? continuationToken = null,
            bool returnTotal = false,
            CancellationToken cancellationToken = default)
        {
            Container container = await _containerProvider.GetContainerAsync()
                .ConfigureAwait(false);

            QueryRequestOptions options = new()
            {
                MaxItemCount = pageSize
            };

            IQueryable<TItem> query = container
                .GetItemLinqQueryable<TItem>(requestOptions: options, continuationToken: continuationToken)
                .Where(_repositoryExpressionProvider.Build(
                    predicate ??
                    _repositoryExpressionProvider.Default<TItem>()));

            _logger.LogQueryConstructed(query);

            Response<int>? countResponse = null;

            if (returnTotal)
            {
                countResponse = await query.CountAsync(cancellationToken);
            }

            (List<TItem> Items, double Charge, string? ContinuationToken) result =
                await GetAllItemsAsync(query, pageSize, cancellationToken);

            _logger.LogQueryExecuted(query, result.Charge);

            return new Page<TItem>(
                countResponse?.Resource ?? null,
                pageSize,
                result.Items.AsReadOnly(),
                result.Charge + countResponse?.RequestCharge ?? 0,
                result.ContinuationToken);
        }

        /// <inheritdoc/>
        public async ValueTask<IPageQueryResult<TItem>> PageAsync(
            Expression<Func<TItem, bool>>? predicate = null,
            int pageNumber = 1,
            int pageSize = 25,
            bool returnTotal = false,
            CancellationToken cancellationToken = default)
        {
            Container container = await _containerProvider.GetContainerAsync()
                .ConfigureAwait(false);

            IQueryable<TItem> query = container
                .GetItemLinqQueryable<TItem>()
                .Where(_repositoryExpressionProvider
                    .Build(predicate ?? _repositoryExpressionProvider.Default<TItem>()));

            Response<int>? countResponse = null;

            if (returnTotal)
            {
                countResponse = await query.CountAsync(cancellationToken);
            }

            query = query.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);

            _logger.LogQueryConstructed(query);

            (List<TItem> Items, double Charge, string? ContinuationToken) result =
                await GetAllItemsAsync(query, pageSize, cancellationToken);

            _logger.LogQueryExecuted(query, result.Charge);

            return new PageQueryResult<TItem>(
                countResponse?.Resource ?? null,
                pageNumber,
                pageSize,
                result.Items.AsReadOnly(),
                result.Charge + countResponse?.RequestCharge ?? 0);
        }
    }
}
