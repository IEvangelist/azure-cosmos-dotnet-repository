// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal sealed partial class DefaultRepository<TItem>
{
    /// <inheritdoc/>
    public async ValueTask<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        Container container =
            await _containerProvider.GetContainerAsync()
                .ConfigureAwait(false);

        IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>();

        TryLogDebugDetails(_logger, () => $"Read: {query}");

        return await _cosmosQueryableProcessor.CountAsync(query, cancellationToken);
    }

    private async ValueTask<Response<int>> CountAsync<TResult>(
        ISpecification<TItem, TResult> specification,
        CancellationToken cancellationToken = default)
        where TResult : IQueryResult<TItem>
    {
        Container container =
            await _containerProvider.GetContainerAsync()
                .ConfigureAwait(false);

        IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>();

        query = _specificationEvaluator.GetQuery(query, specification, evaluateCriteriaOnly: true);

        TryLogDebugDetails(_logger, () => $"Read: {query}");
        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<int> CountAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await _containerProvider.GetContainerAsync()
                .ConfigureAwait(false);

        IQueryable<TItem> query =
            container.GetItemLinqQueryable<TItem>()
                .Where(_repositoryExpressionProvider.Build(predicate));

        TryLogDebugDetails(_logger, () => $"Read: {query}");

        return await _cosmosQueryableProcessor.CountAsync(
            query, cancellationToken);
    }
}