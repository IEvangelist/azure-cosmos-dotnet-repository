// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Exceptions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository
{
    internal partial class DefaultRepository<TItem>
    {
        /// <inheritdoc />
        public async ValueTask UpdateAsBatchAsync(
            IEnumerable<TItem> items,
            CancellationToken cancellationToken = default)
        {
            List<TItem> list = items.ToList();

            if (!list.Any())
            {
                throw new ArgumentException(
                    "Unable to perform batch update with no items",
                    nameof(items));
            }

            string partitionKey = list[0].PartitionKey;

            Container container = await _containerProvider.GetContainerAsync();

            TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            foreach (TItem item in list)
            {
                TransactionalBatchItemRequestOptions options = new();

                if (item is IItemWithEtag itemWithEtag)
                {
                    options.IfMatchEtag = itemWithEtag.Etag;
                }

                batch.UpsertItem(item, options);
            }

            using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new BatchOperationException<TItem>(response);
            }
        }

        /// <inheritdoc />
        public async ValueTask CreateAsBatchAsync(
            IEnumerable<TItem> items,
            CancellationToken cancellationToken = default)
        {
            List<TItem> list = items.ToList();

            if (!list.Any())
            {
                throw new ArgumentException(
                    "Unable to perform batch create operation with no items",
                    nameof(items));
            }

            string partitionKey = list[0].PartitionKey;

            Container container = await _containerProvider.GetContainerAsync();

            TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            foreach (TItem item in list)
            {
                TransactionalBatchItemRequestOptions options = new();
                batch.CreateItem(item);
            }

            using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new BatchOperationException<TItem>(response);
            }
        }
    }
}