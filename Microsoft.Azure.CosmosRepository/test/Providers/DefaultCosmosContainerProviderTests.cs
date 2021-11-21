// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosContainerProviderTests
    {
        readonly Mock<ICosmosContainerService> _cosmosContainerService = new();
        readonly Mock<Container> _container = new();

        [Fact]
        public async Task GetContainerAsyncGetsCorrectContainer()
        {
            ICosmosContainerProvider<TestItem> provider = new DefaultCosmosContainerProvider<TestItem>(_cosmosContainerService.Object);

            _cosmosContainerService.Setup(o => o.GetContainerAsync<TestItem>(false)).ReturnsAsync(_container.Object);

            Container container = await provider.GetContainerAsync();

            Assert.Equal(_container.Object, container);
        }
    }
}