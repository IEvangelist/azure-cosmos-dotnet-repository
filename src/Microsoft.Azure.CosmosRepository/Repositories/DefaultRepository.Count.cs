// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal sealed partial class DefaultRepository<TItem>
{
    /// <inheritdoc/>
    public async ValueTask<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return await InternalCountAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<int> CountAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await InternalCountAsync(predicate, default, cancellationToken);
    }

    public async ValueTask<int> CountAsync(
        Expression<Func<TItem, bool>> predicate,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        return await InternalCountAsync(predicate, partitionKey, cancellationToken);
    }

    public async ValueTask<int> CountAsync(
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        return await InternalCountAsync(null, partitionKey, cancellationToken);
    }

    private async ValueTask<Response<int>> CountAsync<TResult>(
        ISpecification<TItem, TResult> specification,
        CancellationToken cancellationToken = default)
        where TResult : IQueryResult<TItem>
    {
        Container container =
            await containerProvider.GetContainerAsync()
                .ConfigureAwait(false);

        QueryRequestOptions options = new();

        if (specification.PartitionKey is not null)
        {
            options.PartitionKey = specification.PartitionKey;
        }

        IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>(
            requestOptions: options,
            linqSerializerOptions: optionsMonitor.CurrentValue.SerializationOptions);

        query = specificationEvaluator.GetQuery(query, specification, evaluateCriteriaOnly: true);

        TryLogDebugDetails(logger, () => $"Read: {query}");
        return await query.CountAsync(cancellationToken);
    }

    private async ValueTask<int> InternalCountAsync(
        Expression<Func<TItem, bool>>? predicate = null,
        PartitionKey partitionKey = default,
        CancellationToken cancellationToken = default)
    {
        Container container =
            await containerProvider.GetContainerAsync()
                .ConfigureAwait(false);

        QueryRequestOptions requestOptions = new ();

        if (partitionKey != default)
        {
            requestOptions.PartitionKey = partitionKey;
        }

        IQueryable<TItem> query =
            container.GetItemLinqQueryable<TItem>(
                    requestOptions: requestOptions,
                    linqSerializerOptions: optionsMonitor.CurrentValue.SerializationOptions);

        if (predicate is not null)
        {
            query = query.Where(repositoryExpressionProvider.Build(predicate));
        }

        TryLogDebugDetails(logger, () => $"Read: {query}");

        return await cosmosQueryableProcessor.CountAsync(
            query, cancellationToken);
    }

    //TODO: Write docs
    public async ValueTask<int> CountAsync(Expression<Func<TItem, bool>> predicate, IEnumerable<string> partitionKeyValues, CancellationToken cancellationToken = default)
    {
        return await CountAsync(predicate, BuildPartitionKey(partitionKeyValues), cancellationToken);
    }
}