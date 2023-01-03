// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed;

/// <summary>
/// Allows for change feed monitoring.
/// </summary>
public interface IChangeFeedService
{
    /// <summary>
    /// Starts all configured change feed processors.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops all configured change feed processors.
    /// </summary>
    /// <returns></returns>
    Task StopAsync();
}