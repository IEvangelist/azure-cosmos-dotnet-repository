// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Services
{
    /// <summary>
    /// Responsible for providing functions to work with containers
    /// </summary>
    public interface ICosmosContainerService
    {
        /// <summary>
        /// Gets a container for an <see cref="IItem"/>
        /// </summary>
        /// <param name="forceContainerSync"></param>
        Task<Container> GetContainerAsync<TItem>(bool forceContainerSync = false) where TItem : IItem;

        /// <summary>
        /// Gets a container for a set of <see cref="IItem"/>'s
        /// </summary>
        /// <param name="itemTypes">The collection of items that share a container.</param>
        Task<Container> GetContainerAsync(IReadOnlyList<Type> itemTypes);
    }
}