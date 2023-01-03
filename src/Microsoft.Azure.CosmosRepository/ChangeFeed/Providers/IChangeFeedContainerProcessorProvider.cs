// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

/// <summary>
/// Allows an external library to provide its own set of <see cref="IContainerChangeFeedProcessor"/>'s
/// </summary>
public interface IChangeFeedContainerProcessorProvider
{
    /// <summary>
    /// Provides a set of <see cref="IContainerChangeFeedProcessor"/>'s
    /// </summary>
    IEnumerable<IContainerChangeFeedProcessor> GetProcessors();
}