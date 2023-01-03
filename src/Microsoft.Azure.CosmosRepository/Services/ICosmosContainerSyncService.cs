// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Services;

/// <summary>
/// Allows containers properties to be sunk.
/// </summary>
public interface ICosmosContainerSyncService
{
    /// <summary>
    /// Syncs a specific container for an <see cref="IItem"/>
    /// </summary>
    /// <typeparam name="TItem">The <see cref="IItem"/> to sync.</typeparam>
    /// <returns></returns>
    Task SyncContainerPropertiesAsync<TItem>() where TItem : IItem;
}