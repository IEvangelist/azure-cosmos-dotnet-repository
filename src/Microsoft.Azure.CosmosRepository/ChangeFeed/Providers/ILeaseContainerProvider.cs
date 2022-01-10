// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers
{
    interface ILeaseContainerProvider
    {
        Task<Container> GetLeaseContainerAsync();
    }
}