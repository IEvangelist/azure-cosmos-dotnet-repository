// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public interface IEtagMappedRepository<TItem, TMapped> :
    IEtagMappedWriteOnlyRepository<TItem, TMapped>,
    IEtagMappedReadOnlyRepository<TItem, TMapped>
    where TItem : IItemWithEtag
{
}

public interface IEtagMappedWriteOnlyRepository<TItem, TMapped> :
    IDisposable
    where TItem : IItemWithEtag
{

}

public interface IEtagMappedReadOnlyRepository<TItem, TMapped> :
    IDisposable
    where TItem : IItemWithEtag
{
    ValueTask<TMapped?> TryGetAsync(
            string id,
            string? partitionKeyValue = null,
            CancellationToken cancellationToken = default);

    ValueTask<TMapped> GetAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default);

    ValueTask<TMapped> GetAsync(
        string id,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default);

    ValueTask<IEnumerable<TMapped>> GetAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default);
}