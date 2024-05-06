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

    public ValueTask<IPage<TItem>> PageAsync(QueryRequestOptions requestOptions, Expression<Func<TItem, bool>>? predicate = null, int pageSize = 25, string? continuationToken = null, bool returnTotal = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TResult> QueryAsync<TResult>(ISpecification<TItem, TResult> specification, QueryRequestOptions requestOptions, CancellationToken cancellationToken = default) where TResult : IQueryResult<TItem>
    {
        throw new NotImplementedException();
    }

    public ValueTask<IPageQueryResult<TItem>> PageAsync(QueryRequestOptions requestOptions, Expression<Func<TItem, bool>>? predicate = null, int pageNumber = 1, int pageSize = 25, bool returnTotal = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}