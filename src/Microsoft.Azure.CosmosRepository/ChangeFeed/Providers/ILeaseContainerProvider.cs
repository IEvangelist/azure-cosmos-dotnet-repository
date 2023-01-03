// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

/// <summary>
/// Provides access to the lease container.
/// </summary>
public interface ILeaseContainerProvider
{
    /// <summary>
    /// Gets the lease container.
    /// </summary>
    /// <returns>A <see cref="Container"/> instance that represents the lease container</returns>
    Task<Container> GetLeaseContainerAsync();
}