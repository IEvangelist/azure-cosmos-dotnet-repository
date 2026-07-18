// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.CosmosRepository.AspNetCore;

/// <summary>
/// A <see cref="BackgroundService"/> to start and stop the <see cref="IChangeFeedService"/>
/// </summary>
/// <remarks>
/// Creates an instance of the <see cref="CosmosRepositoryChangeFeedHostedService"/>
/// </remarks>
/// <param name="changeFeedService">The <see cref="IChangeFeedService"/> to start.</param>
public class CosmosRepositoryChangeFeedHostedService(IChangeFeedService changeFeedService) : BackgroundService
{
    /// <summary>
    /// Starts the <see cref="IChangeFeedService"/>.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task StartAsync(CancellationToken cancellationToken) =>
        changeFeedService.StartAsync(cancellationToken);

    /// <summary>
    /// Stops the <see cref="IChangeFeedService"/>.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task StopAsync(CancellationToken cancellationToken) =>
        changeFeedService.StopAsync();

    /// <summary>
    /// Background execution is not used because startup and shutdown are explicitly
    /// delegated to the <see cref="IChangeFeedService"/>.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.CompletedTask;
}
