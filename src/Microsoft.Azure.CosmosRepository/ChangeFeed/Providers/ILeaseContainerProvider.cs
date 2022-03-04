// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers
{
    /// <summary>
    /// Provides access to the lease container.
    /// </summary>
    public interface ILeaseContainerProvider
    {
        /// <summary>
        /// Gets the lease container.
        /// </summary>
        /// <returns>A <see cref="Container"/> instance that represents the lease container</returns>
        Task<Container> GetLeaseContainerAsync();
    }
}