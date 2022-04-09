// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Builders;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal partial class ImprovedInMemoryRepository<TItem>
    {
        public ValueTask<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default,
            bool ignoreEtag = false) =>
            _itemStore.UpsertAsync(value, ignoreEtag, cancellationToken);

        public async ValueTask<IEnumerable<TItem>> UpdateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = default, bool ignoreEtag = false)
        {
            IEnumerable<Task<TItem>> updateTasks =
                values.Select(value => UpdateAsync(value, cancellationToken, ignoreEtag).AsTask())
                    .ToList();

            await Task.WhenAll(updateTasks).ConfigureAwait(false);

            return updateTasks.Select(x => x.Result);
        }
    }
}