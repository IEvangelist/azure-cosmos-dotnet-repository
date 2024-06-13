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

    /// <summary>
    /// Builds a partition key from the first item in a list. This method assumes all items in the list belong to the same partition.
    /// Throws an exception if the list is empty.
    /// </summary>
    /// <param name="items">A list of items from which the partition key is to be derived.</param>
    /// <returns>A PartitionKey object constructed from the first item in the list.</returns>
    /// <exception cref="ArgumentException">Thrown when the items list is empty.</exception>
    internal static PartitionKey BuildPartitionKey(List<TItem> items)
    {
        if (items.Count is 0)
        {
            throw new ArgumentException(
                "Unable to perform batch operation with no items",
                nameof(items));
        }
        return BuildPartitionKey(items[0]);
    }

    /// <summary>
    /// Constructs a partition key from a collection of string values. If the collection is empty or null, 
    /// the method uses the defaultValue, if provided, to construct the PartitionKey.
    /// </summary>
    /// <param name="values">An IEnumerable collection of string values for constructing the partition key.</param>
    /// <param name="defaultValue">An optional default value used to construct the PartitionKey if values are null or empty.</param>
    /// <exception cref="ArgumentException">Thrown when the number of provided partition key values exceeds 3.</exception>
    /// <returns>A PartitionKey object constructed from the values or the defaultValue if values are empty or null.</returns>
    internal static PartitionKey BuildPartitionKey(IEnumerable<string> values, string? defaultValue = null)
    {
        var builder = new PartitionKeyBuilder();
        var keys = values?.ToList();
        if (keys is null or { Count: 0 })
        {
            return !string.IsNullOrWhiteSpace(defaultValue)
                ? new PartitionKey(defaultValue)
                : default;
        }

        if (keys?.Count > 3)
        {
            throw new ArgumentException(
                "Unable to build partition key. The max allowed number of partition key values is 3.", 
                nameof(values));
        }


        foreach (var value in values!)
        {
            builder.Add(value);
        }

        return builder.Build();
    }

    /// <summary>
    /// Creates a partition key from a single string value.
    /// </summary>
    /// <param name="value">The string value to use for constructing the partition key.</param>
    /// <returns>A PartitionKey object constructed from the provided string value.</returns>
    internal static PartitionKey BuildPartitionKey(string? value, string? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(defaultValue))
        {
            return new PartitionKey(defaultValue);
        }
        return new PartitionKey(value);
    }

    /// <summary>
    /// Retrieves a partition key from an item by extracting its partition keys and using them to construct a new PartitionKey.
    /// Throws an exception if the item is null.
    /// </summary>
    /// <param name="item">The item from which to extract the partition keys.</param>
    /// <returns>A PartitionKey object constructed from the item's partition keys.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided item is null.</exception>
    internal static PartitionKey BuildPartitionKey(TItem item)
    {
        if (item is null)
        {
            throw new ArgumentException(
                "Unable to perform operation with null item",
                nameof(item));
        }

        return BuildPartitionKey(item.PartitionKeys);
    }

    public ValueTask UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, IEnumerable<string> partitionKeyValues, string? etag = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, PartitionKey partitionKey, string? etag = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}