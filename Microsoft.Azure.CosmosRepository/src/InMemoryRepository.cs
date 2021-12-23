// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Exceptions;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Internals;
using Microsoft.Azure.CosmosRepository.Paging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository
{
    /// <inheritdoc/>
    internal class InMemoryRepository<TItem> : IRepository<TItem> where TItem : class, IItem
    {
        internal long CurrentTs => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        internal ConcurrentDictionary<string, string> Items { get; } = new();

        internal string SerializeItem(TItem item, string etag = null, long? ts = null)
        {
            JObject jObject = JObject.FromObject(item);
            if (etag != null)
            {
                jObject["_etag"] = JToken.FromObject(etag);
            }

            if (ts.HasValue)
            {
                jObject["_ts"] = JToken.FromObject(ts);
            }

            return jObject.ToString();
        }

        internal TItem DeserializeItem(string jsonItem) => JsonConvert.DeserializeObject<TItem>(jsonItem);
        internal TDeserializeTo DeserializeItem<TDeserializeTo>(string jsonItem) => JsonConvert.DeserializeObject<TDeserializeTo>(jsonItem);

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

            TItem item = Items.Values.Select(DeserializeItem).FirstOrDefault(i => i.Id == id && new PartitionKey(i.PartitionKey) == partitionKey);

            if (item is null)
            {
                NotFound();
            }

            return item is { Type: { Length: 0 } } || item?.Type == typeof(TItem).Name ? item : default;
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return Items.Values.Select(DeserializeItem).Where(predicate.Compose(
                item => item.Type == typeof(TItem).Name, Expression.AndAlso).Compile());
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
            value.Id ??= Guid.NewGuid().ToString();

            await Task.CompletedTask;

            TItem item = Items.Values.Select(DeserializeItem).FirstOrDefault(i => i.Id == value.Id && i.PartitionKey == value.PartitionKey);

            if (item is not null)
            {
                Conflict();
            }

            if (value is IItemWithTimeStamps { CreatedTimeUtc: null } valueWithTimestamps)
            {
                valueWithTimestamps.CreatedTimeUtc = DateTime.UtcNow;
            }

            string serialisedValue = SerializeItem(value, Guid.NewGuid().ToString(), CurrentTs);
            Items.TryAdd(value.Id, serialisedValue);

            value = DeserializeItem(Items[value.Id]);
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
        public async ValueTask<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default, bool ignoreEtag = false)
        {
            await Task.CompletedTask;

            if (value is IItemWithEtag valueWithEtag &&
                Items.ContainsKey(value.Id) &&
                DeserializeItem(Items[value.Id]) is IItemWithEtag existingItemWithEtag &&
                !ignoreEtag
                && valueWithEtag.Etag != existingItemWithEtag.Etag)
            {
                MismatchedEtags();
            }

            Items[value.Id] = SerializeItem(value, Guid.NewGuid().ToString(), CurrentTs);

            return DeserializeItem(Items[value.Id]);
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> UpdateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = default, bool ignoreEtag = false)
        {
            IEnumerable<TItem> enumerable = values.ToList();

            foreach (TItem value in enumerable)
            {
                await UpdateAsync(value, cancellationToken, ignoreEtag);
            }

            return enumerable;
        }

        /// <inheritdoc/>
        public async ValueTask UpdateAsync(string id,
            Action<IPatchOperationBuilder<TItem>> builder,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default,
            string etag = default)
        {
            await Task.CompletedTask;

            partitionKeyValue ??= id;

            TItem item = Items.Values.Select(DeserializeItem).FirstOrDefault(x => x.Id == id && x.PartitionKey == partitionKeyValue);
            if (item is null)
            {
                NotFound();
            }

            if (item is IItemWithEtag itemWithEtag &&
                etag != default &&
                !string.IsNullOrWhiteSpace(etag) &&
                itemWithEtag.Etag != etag)
            {
                MismatchedEtags();
            }

            PatchOperationBuilder<TItem> patchOperationBuilder = new();

            builder(patchOperationBuilder);

            foreach (InternalPatchOperation internalPatchOperation in
                patchOperationBuilder._rawPatchOperations.Where(ipo => ipo.Type is PatchOperationType.Replace))
            {

                PropertyInfo property = item!.GetType().GetProperty(internalPatchOperation.PropertyInfo.Name);
                property?.SetValue(item, internalPatchOperation.NewValue);
            }

            Items[id] = SerializeItem(item, Guid.NewGuid().ToString(), CurrentTs);
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

            TItem item = Items.Values.Select(DeserializeItem).FirstOrDefault(i => i.Id == id && new PartitionKey(i.PartitionKey) == partitionKey);

            if (item is null)
            {
                NotFound();
            }

            Items.TryRemove(item!.Id, out _);
        }

        /// <inheritdoc/>
        public ValueTask<bool> ExistsAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = default)
            => ExistsAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return Items.Values.Select(DeserializeItem).FirstOrDefault(i => i.Id == id && new PartitionKey(i.PartitionKey) == partitionKey) is not null;
        }

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return Items.Values.Select(DeserializeItem).Any(predicate.Compose(
                item => item.Type == typeof(TItem).Name, Expression.AndAlso).Compile());
        }

        public ValueTask<IPage<TItem>> PageAsync(
            Expression<Func<TItem, bool>> predicate = null,
            int pageSize = 25,
            string continuationToken = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<IPage<TItem>> PageAsync(Expression<Func<TItem, bool>> predicate = null, int pageNumber = 1, int pageSize = 25, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            IEnumerable<TItem> filteredItems = Items.Values.Select(DeserializeItem)
                                                           .Where(predicate.Compose(item => item.Type == typeof(TItem).Name, Expression.AndAlso).Compile());
            IEnumerable<TItem> items = filteredItems.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            return new Page<TItem>(
                    filteredItems.Count(),
                    pageNumber,
                    pageSize,
                    items.ToList().AsReadOnly(),
                    0);
        }

        private void NotFound() => throw new CosmosException(string.Empty, HttpStatusCode.NotFound, 0, string.Empty, 0);
        private void Conflict() => throw new CosmosException(string.Empty, HttpStatusCode.Conflict, 0, string.Empty, 0);
        private void MismatchedEtags() => throw new CosmosException(string.Empty, HttpStatusCode.PreconditionFailed, 0, string.Empty, 0);

    }
}
