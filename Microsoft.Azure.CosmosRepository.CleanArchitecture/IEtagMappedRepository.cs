// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

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
}