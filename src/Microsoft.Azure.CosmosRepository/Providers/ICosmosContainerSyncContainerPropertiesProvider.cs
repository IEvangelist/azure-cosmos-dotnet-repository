// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// Decides whether or not a given container should sync it's properties.
/// </summary>
interface ICosmosContainerSyncContainerPropertiesProvider
{
    /// <summary>
    /// Gets whether the container should sync container properties.
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="IItem"/></typeparam>
    /// <remarks>If the SyncAllContainerProperties is set to true this will override any specific containers configuration.</remarks>
    /// <returns>Whether or not to sync container properties</returns>
    bool GetWhetherToSyncContainerProperties<TItem>() where TItem : IItem;

    bool GetWhetherToSyncContainerProperties(Type itemType);
}