// Copyright (c) David Pine. All rights reserved.
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
        Container container = await containerProvider.GetContainerAsync()
            .ConfigureAwait(false);

        QueryRequestOptions options = new();

        if (specification.UseContinuationToken)
            options.MaxItemCount = specification.PageSize;
        
        if (specification.PartitionKey is not null)
            options.PartitionKey = specification.PartitionKey;
        

        IQueryable<TItem> query = container
            .GetItemLinqQueryable<TItem>(
                requestOptions: options,
                continuationToken: specification.ContinuationToken, 
                linqSerializerOptions: optionsMonitor.CurrentValue.SerializationOptions)
            .Where(repositoryExpressionProvider.Default<TItem>());

        query = specificationEvaluator.GetQuery(query, specification);


        logger.LogQueryConstructed(query);

        (List<TItem> items, var charge, var continuationToken) =
            await GetAllItemsAsync(query, specification.PageSize, cancellationToken)
                .ConfigureAwait(false);

        logger.LogQueryExecuted(query, charge);

        Response<int> count = await CountAsync(specification, cancellationToken)
            .ConfigureAwait(false);

        return specification.PostProcessingAction(
            items.AsReadOnly(), count.Resource, charge + count.RequestCharge,
            continuationToken);
    }
}
