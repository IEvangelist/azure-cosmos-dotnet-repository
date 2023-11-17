// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    public async ValueTask<TResult> QueryAsync<TResult>(
        ISpecification<TItem, TResult> specification,
        CancellationToken cancellationToken = default)
        where TResult : IQueryResult<TItem>
    {
#if NET7_0_OR_GREATER
        await ValueTask.CompletedTask;
#else
        await Task.CompletedTask;
#endif

        if (specification.UseContinuationToken)
        {
            throw new NotImplementedException();
        }

        IQueryable<TItem> query = InMemoryStorage
            .GetValues<TItem>()
            .Select(DeserializeItem).AsQueryable()
            .Where(item => item.Type == typeof(TItem).Name);

        query = _specificationEvaluator.GetQuery(query, specification);

        var countResponse = query.Count();

        return _specificationEvaluator.GetResult(
            query.ToList().AsReadOnly(),
            specification,
            countResponse,
            0,
            "");
    }
}