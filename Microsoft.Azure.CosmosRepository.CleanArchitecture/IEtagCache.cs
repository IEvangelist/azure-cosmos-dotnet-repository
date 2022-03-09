// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

internal interface IEtagCache
{
    string? RetrieveEtag(IItemWithEtag item);
    void StoreEtag(IItemWithEtag item);
}