// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository
{
    internal sealed partial class DefaultRepository<TItem>
    {
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
    }
}
