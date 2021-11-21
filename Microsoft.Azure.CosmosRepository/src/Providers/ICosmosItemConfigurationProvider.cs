// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <summary>
    /// Holds all of the configuration information for an item.
    /// </summary>
    interface ICosmosItemConfigurationProvider
    {
        ItemOptions GetOptions<TItem>() where TItem : IItem;
    }
}