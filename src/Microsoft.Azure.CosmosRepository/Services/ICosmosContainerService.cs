// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Services;

/// <summary>
/// Responsible for providing functions to work with containers
/// </summary>
public interface ICosmosContainerService
{
    /// <summary>
    /// Gets a container for an <see cref="IItem"/>
    /// </summary>
    /// <param name="forceContainerSync"></param>
    /// <returns>A Cosmos DB <see cref="Container"/></returns>
    Task<Container> GetContainerAsync<TItem>(bool forceContainerSync = false) where TItem : IItem;

    /// <summary>
    /// Gets a container for the given <see cref="IItem"/>'s types.
    /// </summary>
    /// <param name="itemTypes">The types of <see cref="IItem"/>'s</param>
    /// <exception cref="InvalidOperationException">This is thrown when any of the item types do not implement <see cref="IItem"/></exception>
    /// <returns>A Cosmos DB <see cref="Container"/></returns>
    Task<Container> GetContainerAsync(IReadOnlyList<Type> itemTypes);
}