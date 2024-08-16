// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// The cosmos container provider exposes a means of providing
/// an instance to the configured <see cref="Container"/> object.
/// </summary>
public interface ICosmosContainerProvider<TItem> where TItem : IItem
{
    /// <summary>
    /// Asynchronously gets the configured <see cref="Container"/> instance that corresponds to the
    /// cosmos <see cref="RepositoryOptions"/>.
    /// </summary>
    /// <returns>A <see cref="Container"/> instance.</returns>
    Task<Container> GetContainerAsync();
}