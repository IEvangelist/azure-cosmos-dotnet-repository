// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public class EtagCache : IEtagCache
{
    private Dictionary<string, string?> _idToEtagMap = new ();

    public string? GetEtag(string id) =>
        _idToEtagMap.TryGetValue(id, out string? etag) ? etag : null;

    public void SetEtag(string id, string? etag)
    {
        _idToEtagMap[id] = etag;
    }
}