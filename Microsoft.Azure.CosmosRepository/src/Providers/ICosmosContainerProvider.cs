// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <summary>
    /// The cosmos container provider exposes a means of providing
    /// an instance to the configured <see cref="Container"/> object.
    /// </summary>
    interface ICosmosContainerProvider<TItem> where TItem : Item
    {
        /// <summary>
        /// Asynchronously gets the configured <see cref="Container"/> instance that corresponds to the 
        /// cosmos <see cref="RepositoryOptions"/>.
        /// </summary>
        /// <returns>A <see cref="Container"/> instance.</returns>
        Task<Container> GetContainerAsync();
    }
}