// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Specification;

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository
{
    internal partial class InMemoryRepository<TItem>
    {
        public async ValueTask<TResult> QueryAsync<TResult>(
            ISpecification<TItem, TResult> specification,
            CancellationToken cancellationToken = default)
            where TResult : IQueryResult<TItem>
        {
            await Task.CompletedTask;

            if (specification.UseContinuationToken)
            {
                throw new NotImplementedException();
            }

            IQueryable<TItem> query = Items.Values
                .Select(DeserializeItem).AsQueryable()
                .Where(item => item.Type == typeof(TItem).Name);

            query = _specificationEvaluator.GetQuery(query, specification);

            int countResponse = query.Count();

            return _specificationEvaluator.GetResult(
                query.ToList().AsReadOnly(),
                specification,
                countResponse,
                0,
                "");
        }
    }
}