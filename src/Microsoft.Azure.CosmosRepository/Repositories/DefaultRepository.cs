// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

/// <inheritdoc/>
internal sealed partial class DefaultRepository<TItem>(
    IOptionsMonitor<RepositoryOptions> optionsMonitor,
    ICosmosContainerProvider<TItem> containerProvider,
    ILogger<DefaultRepository<TItem>> logger,
    ICosmosQueryableProcessor cosmosQueryableProcessor,
    IRepositoryExpressionProvider repositoryExpressionProvider,
    ISpecificationEvaluator specificationEvaluator) : IRepository<TItem>
    where TItem : IItem
{
    private (bool OptimizeBandwidth, ItemRequestOptions Options) RequestOptions =>
        (optionsMonitor.CurrentValue.OptimizeBandwidth, new ItemRequestOptions
        {
            EnableContentResponseOnWrite = !optionsMonitor.CurrentValue.OptimizeBandwidth
        });

    private static void TryLogDebugDetails(ILogger logger, Func<string> getMessage)
    {
        // ReSharper disable once ConstantConditionalAccessQualifier
        if (logger?.IsEnabled(LogLevel.Debug) ?? false)
        {
            logger.LogDebug("{Msg}", getMessage());
        }
    }

    private static async Task<(List<TItem> items, double charge, string? continuationToken)> GetAllItemsAsync(
        IQueryable<TItem> query,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        string? continuationToken = null;
        List<TItem> results = [];
        var readItemsCount = 0;
        double charge = 0;
        using var iterator = query.ToFeedIterator();
        while (readItemsCount < pageSize && iterator.HasMoreResults)
        {
            FeedResponse<TItem> next =
                await iterator.ReadNextAsync(cancellationToken)
                    .ConfigureAwait(false);

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

    internal static PartitionKey GetPartitionKey(List<TItem> items)
    {
        if (!items.Any())
        {
            throw new ArgumentException(
                "Unable to perform batch operation with no items",
                nameof(items));
        }
        return GetPartitionKey(items[0]);
    }

    internal static PartitionKey GetPartitionKey(string[] values, string? defaultValue = null)
    {
        var builder = new PartitionKeyBuilder();
        if (values == null || values.Length == 0)
        {
            return !string.IsNullOrEmpty(defaultValue) ? new PartitionKey(defaultValue) : default;
        }

        foreach (var value in values)
        {
            builder.Add(value);
        }

        return builder.Build();
    }

    internal static PartitionKey GetPartitionKey(string value)
    {
        return new PartitionKey(value);
    }


    internal static PartitionKey GetPartitionKey(TItem item)
    {
        if (item == null)
        {
            throw new ArgumentException(
                "Unable to perform operation with null item",
                nameof(item));
        }

        return GetPartitionKey(item.PartitionKeys);
    }
}