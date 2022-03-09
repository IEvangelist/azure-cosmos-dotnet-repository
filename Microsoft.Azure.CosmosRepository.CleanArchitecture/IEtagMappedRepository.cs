// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosRepository.Builders;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public interface IEtagMappedRepository<TItem, TMapped>
    : IDisposable
    where TItem : IItemWithEtag
{
    public string? GetEtag(string id);

    public ValueTask<TMapped> GetAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default);

    ValueTask<TMapped> UpdateAsync(
        TMapped value,
        CancellationToken cancellationToken = default);

    public ValueTask<IEnumerable<TMapped>> UpdateAsync(
        IEnumerable<TMapped> values,
        CancellationToken cancellationToken = default);

    ValueTask<TMapped> UpdateAsync(
        string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default,
        string? etag = default);
}