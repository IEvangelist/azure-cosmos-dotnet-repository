// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.ChangeFeed;

namespace ChangeFeedWeb.Services;

public class ChangeFeedHostedService : BackgroundService
{
    private readonly IChangeFeedService _changeFeedService;

    public ChangeFeedHostedService(IChangeFeedService changeFeedService)
    {
        _changeFeedService = changeFeedService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _changeFeedService.StartAsync(stoppingToken);

        stoppingToken.Register(() => _changeFeedService.StopAsync(stoppingToken).Wait(TimeSpan.FromSeconds(5)));
    }
}