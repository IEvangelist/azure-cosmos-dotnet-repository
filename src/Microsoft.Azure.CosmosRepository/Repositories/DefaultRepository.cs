// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

/// <inheritdoc/>
internal sealed partial class DefaultRepository<TItem> : IRepository<TItem>
    where TItem : IItem
{
    readonly ICosmosContainerProvider<TItem> _containerProvider;
    readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
    readonly ILogger<DefaultRepository<TItem>> _logger;
    readonly ICosmosQueryableProcessor _cosmosQueryableProcessor;
    readonly IRepositoryExpressionProvider _repositoryExpressionProvider;
    readonly ISpecificationEvaluator _specificationEvaluator;

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
        ISpecificationEvaluator specificationEvaluator)
    {
        _optionsMonitor = optionsMonitor;
        _containerProvider = containerProvider;
        _logger = logger;
        _cosmosQueryableProcessor = cosmosQueryableProcessor;
        _repositoryExpressionProvider = repositoryExpressionProvider;
        _specificationEvaluator = specificationEvaluator;
    }

    static void TryLogDebugDetails(ILogger logger, Func<string> getMessage)
    {
        // ReSharper disable once ConstantConditionalAccessQualifier
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
}