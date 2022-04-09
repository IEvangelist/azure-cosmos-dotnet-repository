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
        public ValueTask DeleteAsync(TItem value, CancellationToken cancellationToken = default) =>
            _itemStore.DeleteAsync(value.Id, value.PartitionKey, cancellationToken);
    }
}