// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory;
using Microsoft.Azure.CosmosRepository.Internals;

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository
{
    internal partial class InMemoryRepository<TItem>
    {
        private async ValueTask<TItem> UpdateAsync(
            TItem value,
            bool raiseChanges,
            bool ignoreEtag = false)
        {
            await Task.CompletedTask;

            if (value is IItemWithEtag valueWithEtag &&
                !string.IsNullOrWhiteSpace(valueWithEtag.Etag) &&
                Items.ContainsKey(value.Id) &&
                DeserializeItem(Items[value.Id]) is IItemWithEtag existingItemWithEtag &&
                !ignoreEtag
                && valueWithEtag.Etag != existingItemWithEtag.Etag)
            {
                MismatchedEtags();
            }

            Items[value.Id] = SerializeItem(value, Guid.NewGuid().ToString(), CurrentTs);

            TItem item = DeserializeItem(Items[value.Id]);

            if (raiseChanges)
            {
                Changes?.Invoke(new ChangeFeedItemArgs<TItem>(item));
            }

            return item;
        }

        /// <inheritdoc/>
        public ValueTask<TItem> UpdateAsync(TItem value,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false) =>
            UpdateAsync(value, true, ignoreEtag);

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TItem>> UpdateAsync(IEnumerable<TItem> values,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false)
        {
            IEnumerable<TItem> enumerable = values.ToList();

            List<TItem> results = new();

            foreach (TItem value in enumerable)
            {
                results.Add(await UpdateAsync(value, false, ignoreEtag));
            }

            Changes?.Invoke(new ChangeFeedItemArgs<TItem>(results));

            return results;
        }

        /// <inheritdoc/>
        public async ValueTask UpdateAsync(string id,
            Action<IPatchOperationBuilder<TItem>> builder,
            string? partitionKeyValue = null,
            CancellationToken cancellationToken = default,
            string? etag = default)
        {
            await Task.CompletedTask;

            partitionKeyValue ??= id;

            TItem? item = Items
                .Values
                .Select(DeserializeItem)
                .FirstOrDefault(x => x.Id == id && x.PartitionKey == partitionKeyValue);

            switch (item)
            {
                case null:
                    NotFound();
                    break;
                case IItemWithEtag itemWithEtag when
                    etag != default &&
                    !string.IsNullOrWhiteSpace(etag) &&
                    itemWithEtag.Etag != etag:
                    MismatchedEtags();
                    break;
            }

            PatchOperationBuilder<TItem> patchOperationBuilder = new();

            builder(patchOperationBuilder);

            foreach (InternalPatchOperation internalPatchOperation in
                     patchOperationBuilder._rawPatchOperations.Where(ipo => ipo.Type is PatchOperationType.Replace))
            {
                PropertyInfo property = item!.GetType().GetProperty(internalPatchOperation.PropertyInfo.Name)!;
                property.SetValue(item, internalPatchOperation.NewValue);
            }

            Items[id] = SerializeItem(item!, Guid.NewGuid().ToString(), CurrentTs);

            Changes?.Invoke(new ChangeFeedItemArgs<TItem>(DeserializeItem(Items[id])));
        }

        private void MismatchedEtags() =>
            throw new CosmosException(string.Empty, HttpStatusCode.PreconditionFailed, 0, string.Empty, 0);
    }
}