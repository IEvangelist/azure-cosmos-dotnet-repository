// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

/// <summary>
/// A builder to configure a container for an item
/// </summary>
public interface IItemContainerBuilder
{
    /// <summary>
    /// The list of already configured options for a <see cref="IItem"/>
    /// </summary>
    IReadOnlyList<ContainerOptionsBuilder> Options { get; }

    /// <summary>
    /// Provides a <see cref="ContainerOptionsBuilder"/> instance to configure a container for an item
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="IItem"/> to configure.</typeparam>
    /// <returns>Instance of <see cref="IItemContainerBuilder"/></returns>
    IItemContainerBuilder Configure<TItem>(Action<ContainerOptionsBuilder> containerOptions) where TItem : IItem;
}
