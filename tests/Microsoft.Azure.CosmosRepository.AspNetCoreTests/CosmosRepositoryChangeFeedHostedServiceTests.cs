// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.AspNetCore;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepository.AspNetCoreTests;

public class CosmosRepositoryChangeFeedHostedServiceTests
{
    [Fact]
    public async Task StartAsync_ChangeFeedService_StartsChangeFeedService()
    {
        Mock<IChangeFeedService> changeFeedService = new(MockBehavior.Strict);
        changeFeedService
            .Setup(o => o.StartAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var changeFeedHostedService = new CosmosRepositoryChangeFeedHostedService(changeFeedService.Object);

        await changeFeedHostedService.StartAsync(default);

        changeFeedService.Verify(o => o.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
        changeFeedService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StopAsync_ChangeFeedService_StopsChangeFeedService()
    {
        Mock<IChangeFeedService> changeFeedService = new(MockBehavior.Strict);
        changeFeedService
            .Setup(o => o.StopAsync())
            .Returns(Task.CompletedTask);
        var changeFeedHostedService = new CosmosRepositoryChangeFeedHostedService(changeFeedService.Object);

        await changeFeedHostedService.StopAsync(default);

        changeFeedService.Verify(o => o.StopAsync(), Times.Once);
        changeFeedService.VerifyNoOtherCalls();
    }
}
