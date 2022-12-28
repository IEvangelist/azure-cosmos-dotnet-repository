// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// The cosmos container default time to live provider to an <see cref="IItem"/>
/// </summary>
interface ICosmosContainerDefaultTimeToLiveProvider
{
    /// <summary>
    /// Gets teh default time to live value for an <see cref="IItem"/>
    /// </summary>
    /// <typeparam name="TItem">The <see cref="IItem"/></typeparam>
    /// <remarks>If no options are found for this item then it's default in the container will be set to -1 (live forever).</remarks>
    /// <returns>The time to live in seconds.</returns>
    int GetDefaultTimeToLive<TItem>() where TItem : IItem;

    int GetDefaultTimeToLive(Type itemType);
}