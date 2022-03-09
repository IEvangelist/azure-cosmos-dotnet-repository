// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

internal sealed class EtagCache : IEtagCache
{
    private readonly ConcurrentDictionary<string, string?> _idToEtagMap = new ();

    private string ComputeUid(string id, string partitionKey)
        => $"{id}#{partitionKey}";

    public string? RetrieveEtag(IItemWithEtag item) =>
        _idToEtagMap.TryGetValue(ComputeUid(item.Id, item.PartitionKey), out string? etag) ? etag : null;

    public void StoreEtag(IItemWithEtag item) =>
        _idToEtagMap[ComputeUid(item.Id, item.PartitionKey)] = item.Etag;
}