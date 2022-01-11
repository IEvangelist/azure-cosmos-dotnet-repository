// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed
{
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
}