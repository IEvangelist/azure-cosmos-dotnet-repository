// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.CosmosRepository.AspNetCore;

/// <summary>
/// A <see cref="BackgroundService"/> to start & stop the <see cref="IChangeFeedService"/>
/// </summary>
public class CosmosRepositoryChangeFeedHostedService : BackgroundService
{
    private readonly IChangeFeedService _changeFeedService;

    internal CosmosRepositoryChangeFeedHostedService(IChangeFeedService changeFeedService)
    {
        _changeFeedService = changeFeedService;
    }

    /// <summary>
    /// Start's the <see cref="IChangeFeedService"/>
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await _changeFeedService.StartAsync(stoppingToken);
}