// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository
{
    /// <inheritdoc/>
    public class InMemoryRepository<TItem> : IRepository<TItem> where TItem : IItem
    {
        internal List<TItem> Documents { get; }

        internal InMemoryRepository()
        {
            Documents = new List<TItem>();
        }

        /// <inheritdoc/>
        public ValueTask<TItem> GetAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = default)
            => GetAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<TItem> GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            TItem item = Documents.FirstOrDefault(i => i.Id == id && i.PartitionKey == partitionKey);

            if (item is null)
                throw new CosmosException(string.Empty, HttpStatusCode.NotFound, 0, string.Empty, 0);

            return item is { Type: { Length: 0 } } || item.Type == typeof(TItem).Name ? item : default;
        }

        /// <inheritdoc/>
        public ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<IEnumerable<TItem>> GetByQueryAsync(string query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<IEnumerable<TItem>> GetByQueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<TItem> CreateAsync(TItem value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<IEnumerable<TItem>> CreateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask DeleteAsync(TItem value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask DeleteAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}