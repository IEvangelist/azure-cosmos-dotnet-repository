// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Items;

namespace Aspire.Microsoft.Azure.CosmosRepository;

public interface IWriteOnlyRepository
{
    ValueTask CreateAsync<TItem>(
        TItem item,
        CancellationToken cancellationToken = default)
        where TItem : class, IItem;

    ValueTask CreateOrUpdateAsync<TItem>(
        TItem item,
        CancellationToken cancellationToken = default)
        where TItem : class, IItem;

    ValueTask DeleteAsync<TItem>(
        string id,
        string partitionKey,
        CancellationToken cancellationToken = default)
        where TItem : class, IItem;
}