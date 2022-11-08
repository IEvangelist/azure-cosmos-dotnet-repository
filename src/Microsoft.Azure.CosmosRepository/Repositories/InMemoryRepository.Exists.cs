// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    /// <inheritdoc/>
    public ValueTask<bool> ExistsAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default)
        => ExistsAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<bool> ExistsAsync(
        string id,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        return Items
            .Values
            .Select(DeserializeItem)
            .FirstOrDefault(i => i.Id == id && new PartitionKey(i.PartitionKey) == partitionKey) is not null;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> ExistsAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return Items.Values.Select(DeserializeItem).Any(predicate.Compose(
            item => item.Type == typeof(TItem).Name, Expression.AndAlso).Compile());
    }
}