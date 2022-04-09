// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal partial class ImprovedInMemoryRepository<TItem>
    {
        public ValueTask<TItem> CreateAsync(TItem value, CancellationToken cancellationToken = default) =>
            _itemStore.WriteAsync(value, value.Id, value.PartitionKey, cancellationToken)!;

        public async ValueTask<IEnumerable<TItem>> CreateAsync(
            IEnumerable<TItem> values,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Task<TItem>> creationTasks = values.Select(value =>
                    CreateAsync(value, cancellationToken).AsTask())
                    .ToList();

            await Task.WhenAll(creationTasks).ConfigureAwait(false);

            return creationTasks.Select(x => x.Result!);
        }
    }
}