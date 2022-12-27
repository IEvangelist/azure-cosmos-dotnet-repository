// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal sealed partial class DefaultRepository<TItem>
{
    /// <inheritdoc/>
    public async ValueTask<TResult> QueryAsync<TResult>(
        ISpecification<TItem, TResult> specification,
        CancellationToken cancellationToken = default)
        where TResult : IQueryResult<TItem>
    {
        Container container = await _containerProvider.GetContainerAsync()
            .ConfigureAwait(false);

        QueryRequestOptions options = new();

        if (specification.UseContinuationToken)
        {
            options.MaxItemCount = specification.PageSize;
        }

        IQueryable<TItem> query = container
            .GetItemLinqQueryable<TItem>(
                requestOptions: options,
                continuationToken: specification.ContinuationToken)
            .Where(_repositoryExpressionProvider.Default<TItem>());

        query = _specificationEvaluator.GetQuery(query, specification);

        _logger.LogQueryConstructed(query);

        (List<TItem> items, var charge, var continuationToken) =
            await GetAllItemsAsync(query, specification.PageSize, cancellationToken)
                .ConfigureAwait(false);

        _logger.LogQueryExecuted(query, charge);

        Response<int> count = await CountAsync(specification, cancellationToken)
            .ConfigureAwait(false);

        return specification.PostProcessingAction(
            items.AsReadOnly(), count.Resource, charge + count.RequestCharge,
            continuationToken);
    }
}
