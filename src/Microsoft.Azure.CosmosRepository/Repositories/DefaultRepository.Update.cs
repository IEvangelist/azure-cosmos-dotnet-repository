// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal sealed partial class DefaultRepository<TItem>
{
    /// <inheritdoc/>
    public async ValueTask<TItem> UpdateAsync(
        TItem value,
        bool ignoreEtag = false,
        CancellationToken cancellationToken = default)
    {
        (var optimizeBandwidth, ItemRequestOptions options) = RequestOptions;
        Container container =
            await containerProvider.GetContainerAsync()
                .ConfigureAwait(false);

        if (value is IItemWithEtag valueWithEtag && !ignoreEtag)
        {
            options.IfMatchEtag = string.IsNullOrWhiteSpace(valueWithEtag.Etag)
                ? default
                : valueWithEtag.Etag;
        }

        PartitionKey partitionKey = BuildPartitionKey(value);

        ItemResponse<TItem> response =
            await container.UpsertItemAsync(value, partitionKey, options,
                    cancellationToken)
                .ConfigureAwait(false);

        TryLogDebugDetails(logger, () => $"Updated: {JsonConvert.SerializeObject(value)}");

        return optimizeBandwidth ? value : response.Resource;
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<TItem>> UpdateAsync(
        IEnumerable<TItem> values,
        bool ignoreEtag = false,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Task<TItem>> updateTasks =
            values.Select(value => UpdateAsync(value, ignoreEtag, cancellationToken).AsTask())
                .ToList();

        await Task.WhenAll(updateTasks).ConfigureAwait(false);

        return updateTasks.Select(x => x.Result);
    }

    public async ValueTask UpdateAsync(string id,
    Action<IPatchOperationBuilder<TItem>> builder,
    string? etag = default,
    CancellationToken cancellationToken = default)
    {
        await InternalUpdateAsync(id, builder, null, etag, cancellationToken);
    }

    public async ValueTask UpdateAsync(string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        string partitionKeyValue,
        string? etag = default,
        CancellationToken cancellationToken = default)
    {
        await InternalUpdateAsync(id, builder, BuildPartitionKey(partitionKeyValue, id), etag, cancellationToken);
    }

    public async ValueTask UpdateAsync(string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        IEnumerable<string> partitionKeyValues,
        string? etag = default,
        CancellationToken cancellationToken = default)
    {
        await InternalUpdateAsync(id, builder, BuildPartitionKey(partitionKeyValues, id), etag, cancellationToken);
    }

    public async ValueTask InternalUpdateAsync(string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        PartitionKey? partitionKey = null,
        string? etag = default,
        CancellationToken cancellationToken = default)
    {
        CosmosPropertyNamingPolicy? propertyNamingPolicy =
            optionsMonitor.CurrentValue.SerializationOptions?.PropertyNamingPolicy;
        IPatchOperationBuilder<TItem> patchOperationBuilder = new PatchOperationBuilder<TItem>(propertyNamingPolicy);

        builder(patchOperationBuilder);

        Container container = await containerProvider.GetContainerAsync();

        PatchItemRequestOptions patchItemRequestOptions = new();
        if (etag != default && !string.IsNullOrWhiteSpace(etag))
        {
            patchItemRequestOptions.IfMatchEtag = etag;
        }

        await container.PatchItemAsync<TItem>(id, partitionKey ?? new PartitionKey(id),
            patchOperationBuilder.PatchOperations, patchItemRequestOptions, cancellationToken);
    }
}