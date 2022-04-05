// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed partial class ImprovedInMemoryRepository<TItem> : IRepository<TItem> where TItem : IItem
    {
        private readonly IItemStore<TItem> _itemStore;

        public ImprovedInMemoryRepository(IItemStore<TItem> itemStore)
        {
            _itemStore = itemStore;
        }

        public ValueTask<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default, bool ignoreEtag = false)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<TItem>> UpdateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = default, bool ignoreEtag = false)
        {
            throw new NotImplementedException();
        }

        public ValueTask UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, string? partitionKeyValue = null,
            CancellationToken cancellationToken = default, string? etag = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteAsync(TItem value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<TItem?> TryGetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<TItem> GetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<TItem> GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<TItem>> GetByQueryAsync(string query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<TItem>> GetByQueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> ExistsAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> CountAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IPage<TItem>> PageAsync(Expression<Func<TItem, bool>>? predicate = null, int pageSize = 25, string? continuationToken = null,
            bool returnTotal = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<TResult> QueryAsync<TResult>(ISpecification<TItem, TResult> specification, CancellationToken cancellationToken = default) where TResult : IQueryResult<TItem>
        {
            throw new NotImplementedException();
        }

        public ValueTask<IPageQueryResult<TItem>> PageAsync(Expression<Func<TItem, bool>>? predicate = null, int pageNumber = 1, int pageSize = 25, bool returnTotal = false,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask UpdateAsBatchAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask CreateAsBatchAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteAsBatchAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}