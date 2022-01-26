// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <summary>
    /// Holds all of the configuration information for an item.
    /// </summary>
    interface ICosmosItemConfigurationProvider
    {
        ItemConfiguration GetItemConfiguration<TItem>() where TItem : IItem;

        ItemConfiguration GetItemConfiguration(Type itemType);
    }
}