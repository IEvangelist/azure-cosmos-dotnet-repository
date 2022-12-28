// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// The cosmos container name provider maps container names to
/// <see cref="IItem"/> implementations.
/// </summary>
interface ICosmosContainerNameProvider
{
    /// <summary>
    /// Gets the container name for the corresponding <typeparamref name="TItem"/>.
    /// When decorating <typeparamref name="TItem"/> implementations with the
    /// <see cref="ContainerAttribute"/>, and configuring <see cref="RepositoryOptions.ContainerPerItemType"/>
    /// to <c>true</c>, the container name is used instead of the type name.
    /// </summary>
    /// <returns>The container name.</returns>
    string GetContainerName<TItem>() where TItem : IItem;

    string GetContainerName(Type itemType);
}
