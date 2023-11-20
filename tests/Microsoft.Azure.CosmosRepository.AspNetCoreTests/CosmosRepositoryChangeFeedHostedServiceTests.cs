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
    public async Task ExecuteAsync_ChangeFeedService_StartsChangeFeedService()
    {
        //Arrange
        Mock<IChangeFeedService> changeFeedService = new();
        var changeFeedHostedService = new CosmosRepositoryChangeFeedHostedService(changeFeedService.Object);

        //Act
        await changeFeedHostedService.StartAsync(default);

        //Assert
        changeFeedService.Verify(o => o.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}