// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Builders;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public interface IEtagMappedRepository<TItem, TMapped>
    : IDisposable
    where TItem : IItemWithEtag
    where TMapped : class
{
    public string? Etag { get; }

    public ValueTask<TMapped> GetAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default);

    ValueTask UpdateAsync(
        TMapped value,
        CancellationToken cancellationToken = default);

    ValueTask UpdateAsync(
        string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default,
        string? etag = default);
}