// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Builders;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosRepository
{
    internal sealed partial class DefaultRepository<TItem>
    {
        /// <inheritdoc/>
        public async ValueTask<TItem> UpdateAsync(
            TItem value,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false)
        {
            (bool optimizeBandwidth, ItemRequestOptions options) = RequestOptions;
            Container container =
                await _containerProvider.GetContainerAsync()
                    .ConfigureAwait(false);

            if (value is IItemWithEtag valueWithEtag && !ignoreEtag)
            {
                options.IfMatchEtag = string.IsNullOrWhiteSpace(valueWithEtag.Etag)
                    ? default
                    : valueWithEtag.Etag;
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
    }
}
