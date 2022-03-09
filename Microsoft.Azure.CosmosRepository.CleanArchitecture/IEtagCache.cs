// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public interface IEtagCache
{
    string? GetEtag(string id);
    void SetEtag(string id, string? etag);
}