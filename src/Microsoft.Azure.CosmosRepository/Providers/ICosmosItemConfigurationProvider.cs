// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// Holds all of the configuration information for an item.
/// </summary>
internal interface ICosmosItemConfigurationProvider
{
    ItemConfiguration GetItemConfiguration<TItem>() where TItem : IItem;

    ItemConfiguration GetItemConfiguration(Type itemType);

    List<ItemConfiguration> GetAllItemConfigurations(params Assembly[]? assemblies);
}