// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultCosmosContainerProviderTests
{
    readonly Mock<ICosmosContainerService> _cosmosContainerService = new();
    readonly Mock<Container> _container = new();

    [Fact]
    public async Task GetContainerAsyncGetsCorrectContainer()
    {
        ICosmosContainerProvider<TestItemWithEtag> provider = new DefaultCosmosContainerProvider<TestItemWithEtag>(_cosmosContainerService.Object);

        _cosmosContainerService.Setup(o => o.GetContainerAsync<TestItemWithEtag>(false)).ReturnsAsync(_container.Object);

        Container container = await provider.GetContainerAsync();

        Assert.Equal(_container.Object, container);
    }
}