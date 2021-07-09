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
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository.Extensions;

namespace Microsoft.Azure.CosmosRepository
{
    /// <inheritdoc/>
    public class InMemoryRepository<TItem> : IRepository<TItem> where TItem : IItem
    {
        internal List<TItem> Items { get; } = new();

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

            TItem item = Items.FirstOrDefault(i => i.Id == id && i.PartitionKey == partitionKey);

            if (item is null)
                NotFound();

            return item is { Type: { Length: 0 } } || item.Type == typeof(TItem).Name ? item : default;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return Items.Where(predicate.Compose(
                item =>  item.Type == typeof(TItem).Name, Expression.AndAlso).Compile());
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
        public async ValueTask<TItem> CreateAsync(TItem value, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            TItem item = Items.FirstOrDefault(i => i.Id == value.Id && i.PartitionKey == value.PartitionKey);

            if(item is not null)
                Conflict();

            Items.Add(value);

            return value;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> CreateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = default)
        {
            IEnumerable<TItem> enumerable = values.ToList();

            foreach (TItem value in enumerable)
            {
                await CreateAsync(value, cancellationToken);
            }

            return enumerable;
        }

        /// <inheritdoc/>
        public async ValueTask<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            TItem item = Items.FirstOrDefault(i => i.Id == value.Id && i.PartitionKey == value.PartitionKey);

            if (item is not null)
            {
                Items.Remove(item);
                Items.Add(value);
                return value;
            }

            Items.Add(value);

            return value;
        }

        /// <inheritdoc/>
        public ValueTask DeleteAsync(TItem value, CancellationToken cancellationToken = default)
            => DeleteAsync(value.Id, value.PartitionKey, cancellationToken);

        /// <inheritdoc/>
        public ValueTask DeleteAsync(string id, string partitionKeyValue = null,
            CancellationToken cancellationToken = default)
            => DeleteAsync(id, new PartitionKey(partitionKeyValue), cancellationToken);

        /// <inheritdoc/>
        public async ValueTask DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            if (partitionKey == default)
            {
                partitionKey = new PartitionKey(id);
            }

            TItem item = Items.FirstOrDefault(i => i.Id == id && i.PartitionKey == partitionKey);

            if(item is null)
                NotFound();

            Items.Remove(item);
        }

        private void NotFound() => throw new CosmosException(string.Empty, HttpStatusCode.NotFound, 0, string.Empty, 0);
        private void Conflict() => throw new CosmosException(string.Empty, HttpStatusCode.Conflict, 0, string.Empty, 0);
    }
}