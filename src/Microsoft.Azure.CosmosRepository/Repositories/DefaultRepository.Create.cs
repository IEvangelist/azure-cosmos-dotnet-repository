// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal sealed partial class DefaultRepository<TItem>
{
    /// <inheritdoc/>
    public async ValueTask<TItem> CreateAsync(
        TItem value,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await _containerProvider.GetContainerAsync().ConfigureAwait(false);

        if (value is IItemWithTimeStamps { CreatedTimeUtc: null } valueWithTimestamps)
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
}
