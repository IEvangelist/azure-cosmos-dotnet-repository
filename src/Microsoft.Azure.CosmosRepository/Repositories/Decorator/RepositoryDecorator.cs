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

namespace Microsoft.Azure.CosmosRepository.Decorator
{
    /// <summary>
    /// A decorator to get the underlying repository for read and write only interface dependencies
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    internal class RepositoryDecorator<TItem> :
        IReadOnlyRepository<TItem>,
        IWriteOnlyRepository<TItem>
        where TItem : IItem
    {
        private readonly IRepository<TItem> _repository;

        /// <summary>
        /// Resolves the actual underlying repository
        /// </summary>
        /// <param name="repository"></param>
        public RepositoryDecorator(IRepository<TItem> repository)
        {
            _repository = repository;
        }

        /// <inheritdoc />
        public ValueTask<TItem?> TryGetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            return _repository.TryGetAsync(id, partitionKeyValue, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<TItem> GetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            return _repository.GetAsync(id, partitionKeyValue, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<TItem> GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            return _repository.GetAsync(id, partitionKey, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _repository.GetAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<IEnumerable<TItem>> GetByQueryAsync(string query, CancellationToken cancellationToken = default)
        {
            return _repository.GetByQueryAsync(query, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<IEnumerable<TItem>> GetByQueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
        {
            return _repository.GetByQueryAsync(queryDefinition, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<bool> ExistsAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            return _repository.ExistsAsync(id, partitionKeyValue, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<bool> ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            return _repository.ExistsAsync(id, partitionKey, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _repository.ExistsAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return _repository.CountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<int> CountAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _repository.CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<IPage<TItem>> PageAsync(Expression<Func<TItem, bool>>? predicate = null, int pageSize = 25, string? continuationToken = null,
            bool returnTotal = false, CancellationToken cancellationToken = default)
        {
            return _repository.PageAsync(predicate, pageSize, continuationToken, returnTotal, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<TResult> QueryAsync<TResult>(ISpecification<TItem, TResult> specification, CancellationToken cancellationToken = default) where TResult : IQueryResult<TItem>
        {
            return _repository.QueryAsync(specification, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<IPageQueryResult<TItem>> PageAsync(Expression<Func<TItem, bool>>? predicate = null, int pageNumber = 1, int pageSize = 25, bool returnTotal = false,
            CancellationToken cancellationToken = default)
        {
            return _repository.PageAsync(predicate, pageNumber, pageSize, returnTotal, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<TItem> CreateAsync(TItem value, CancellationToken cancellationToken = default)
        {
            return _repository.CreateAsync(value, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<IEnumerable<TItem>> CreateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = default)
        {
            return _repository.CreateAsync(values, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default, bool ignoreEtag = false)
        {
            return _repository.UpdateAsync(value, cancellationToken, ignoreEtag);
        }

        /// <inheritdoc />
        public ValueTask<IEnumerable<TItem>> UpdateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = default, bool ignoreEtag = false)
        {
            return _repository.UpdateAsync(values, cancellationToken, ignoreEtag);
        }

        /// <inheritdoc />
        public ValueTask UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, string? partitionKeyValue = null,
            CancellationToken cancellationToken = default, string? etag = default)
        {
            return _repository.UpdateAsync(id, builder, partitionKeyValue, cancellationToken, etag);
        }

        /// <inheritdoc />
        public ValueTask DeleteAsync(TItem value, CancellationToken cancellationToken = default)
        {
            return _repository.DeleteAsync(value, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask DeleteAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            return _repository.DeleteAsync(id, partitionKeyValue, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            return _repository.DeleteAsync(id, partitionKey, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask UpdateAsBatchAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
        {
            return _repository.UpdateAsBatchAsync(items, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask CreateAsBatchAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
        {
            return _repository.CreateAsBatchAsync(items, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask DeleteAsBatchAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
        {
            return _repository.DeleteAsBatchAsync(items, cancellationToken);
        }
    }
}