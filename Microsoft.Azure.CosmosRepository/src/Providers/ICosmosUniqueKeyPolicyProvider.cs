// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    interface ICosmosUniqueKeyPolicyProvider
    {
        UniqueKeyPolicy GetUniqueKeyPolicy<TItem>() where TItem : IItem;
    }
}