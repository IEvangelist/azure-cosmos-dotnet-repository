// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Azure.CosmosRepository.Validators;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Services
{
    public class DefaultCosmosContainerServiceTests
    {
        readonly Mock<IOptions<RepositoryOptions>> _options = new ();
        readonly Mock<ICosmosItemConfigurationProvider> _itemConfigurationProvider = new();
        readonly Mock<IRepositoryOptionsValidator> _repositoryOptionsValidator = new();
        readonly Mock<CosmosClient> _cosmosClient = new();
        readonly Mock<Database> _database = new();
        readonly Mock<Container> _container = new();
        readonly Mock<DatabaseResponse> _databaseResponse = new();
        readonly Mock<ContainerResponse> _containerResponse = new();
        readonly RepositoryOptions _repositoryOptions = new();

        public DefaultCosmosContainerServiceTests()
        {
            _databaseResponse.Setup(o => o.Database).Returns(_database.Object);
            _containerResponse.Setup(o => o.Container).Returns(_container.Object);
            _options.Setup(o => o.Value).Returns(_repositoryOptions);
            _containerResponse.Setup(o => o.Container).Returns(_container.Object);
        }

        ICosmosClientProvider GetClientProvider() =>
            new TestCosmosClientProvider(_cosmosClient.Object);

        DefaultCosmosContainerService CreateDefaultCosmosContainerService() =>
            new(
                _itemConfigurationProvider.Object,
                GetClientProvider(),
                _options.Object,
                new NullLogger<DefaultCosmosContainerService>(),
                _repositoryOptionsValidator.Object);


        [Fact]
        public async Task GetContainerAsyncWhenContainerPerItemTypeIsNotSetGetsCorrectContainer()
        {
            //Arrange
            ICosmosContainerService service = CreateDefaultCosmosContainerService();
            _repositoryOptions.ContainerPerItemType = false;
            _repositoryOptions.ContainerId = "containerA";

            ItemOptions itemOptions = new (typeof(TestItem), "a", "/id", new(), ThroughputProperties.CreateManualThroughput(400));

            _itemConfigurationProvider.Setup(o => o.GetOptions<TestItem>()).Returns(itemOptions);

            _cosmosClient.Setup(o =>
                    o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
                .ReturnsAsync(_databaseResponse.Object);

            _database.Setup(o =>
                o.CreateContainerIfNotExistsAsync(
                    It.Is<ContainerProperties>(c => c.Id == "containerA"
                                                    && c.PartitionKeyPath == "/id"),
                    itemOptions.ThroughputProperties,
                    null,
                    CancellationToken.None))
                .ReturnsAsync(_containerResponse.Object);

            //Act
            Container container = await service.GetContainerAsync<TestItem>();

            //Assert
            Assert.Equal(_container.Object, container);
            _container.Verify(o => o.ReplaceContainerAsync(It.IsAny<ContainerProperties>(), null, CancellationToken.None), Times.Never);
            _container.Verify(o => o.ReplaceThroughputAsync(It.IsAny<ThroughputProperties>(), null, CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task GetContainerAsyncWhenContainerPerItemTypeIsSetGetsCorrectContainer()
        {
            //Arrange
            ICosmosContainerService service = CreateDefaultCosmosContainerService();
            _repositoryOptions.ContainerPerItemType = true;

            ItemOptions itemOptions = new (typeof(TestItem), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400) ,5);

            _itemConfigurationProvider.Setup(o => o.GetOptions<TestItem>()).Returns(itemOptions);

            _cosmosClient.Setup(o =>
                    o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
                .ReturnsAsync(_databaseResponse.Object);

            _database.Setup(o =>
                    o.CreateContainerIfNotExistsAsync(
                        It.Is<ContainerProperties>(c => c.Id == "a"
                                                        && c.PartitionKeyPath == "/test"
                                                        && c.DefaultTimeToLive == 5),
                        itemOptions.ThroughputProperties,
                        null,
                        CancellationToken.None))
                .ReturnsAsync(_containerResponse.Object);

            //Act
            Container container = await service.GetContainerAsync<TestItem>();

            //Assert
            Assert.Equal(_container.Object, container);
            _container.Verify(o => o.ReplaceContainerAsync(It.IsAny<ContainerProperties>(), null, CancellationToken.None), Times.Never);
            _container.Verify(o => o.ReplaceThroughputAsync(It.IsAny<ThroughputProperties>(), null, CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task GetContainerAsyncWhenSyncContainerPropertiesIsSetReplacesContainerProperties()
        {
            //Arrange
            ICosmosContainerService service = CreateDefaultCosmosContainerService();
            _repositoryOptions.ContainerPerItemType = true;

            ItemOptions itemOptions = new (typeof(TestItem), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400),5, true);

            _itemConfigurationProvider.Setup(o => o.GetOptions<TestItem>()).Returns(itemOptions);

            _cosmosClient.Setup(o =>
                    o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
                .ReturnsAsync(_databaseResponse.Object);

            _database.Setup(o =>
                    o.CreateContainerIfNotExistsAsync(
                        It.Is<ContainerProperties>(c => ValidateContainerProperties(c)),
                        itemOptions.ThroughputProperties,
                        null,
                        CancellationToken.None))
                .ReturnsAsync(_containerResponse.Object);

            _container.Setup(o => o.Id).Returns("a");

            //Act
            Container container = await service.GetContainerAsync<TestItem>();

            //Assert
            _container.Verify(o => o.ReplaceContainerAsync(It.Is<ContainerProperties>(c => ValidateContainerProperties(c)), null, CancellationToken.None), Times.Once);
            _container.Verify(o => o.ReplaceThroughputAsync(itemOptions.ThroughputProperties, null, CancellationToken.None), Times.Once);

            Assert.Equal(_container.Object, container);
        }

        [Fact]
        public async Task GetContainerAsyncWhenSyncContainerPropertiesIsFalseButForceIsSetContainerProperties()
        {
            //Arrange
            ICosmosContainerService service = CreateDefaultCosmosContainerService();
            _repositoryOptions.ContainerPerItemType = true;

            ItemOptions itemOptions = new (typeof(TestItem), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400),5, false);

            _itemConfigurationProvider.Setup(o => o.GetOptions<TestItem>()).Returns(itemOptions);

            _cosmosClient.Setup(o =>
                    o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
                .ReturnsAsync(_databaseResponse.Object);

            _database.Setup(o =>
                    o.CreateContainerIfNotExistsAsync(
                        It.Is<ContainerProperties>(c => ValidateContainerProperties(c)),
                        itemOptions.ThroughputProperties,
                        null,
                        CancellationToken.None))
                .ReturnsAsync(_containerResponse.Object);

            //Act
            Container container = await service.GetContainerAsync<TestItem>(true);

            //Assert
            _container.Verify(o => o.ReplaceContainerAsync(It.Is<ContainerProperties>(c => ValidateContainerProperties(c)), null, CancellationToken.None), Times.Once);
            _container.Verify(o => o.ReplaceThroughputAsync(itemOptions.ThroughputProperties, null, CancellationToken.None), Times.Once);

            Assert.Equal(_container.Object, container);
        }

        [Fact]
        public async Task GetContainerAsyncWhenSyncContainerPropertiesIsSetAndConatinerHasAlreadyBeenSyncDoesNotSyncContainerAgain()
        {
            //Arrange
            ICosmosContainerService service = CreateDefaultCosmosContainerService();
            _repositoryOptions.ContainerPerItemType = true;

            ItemOptions itemOptions = new (typeof(TestItem), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400),5, true);

            _itemConfigurationProvider.Setup(o => o.GetOptions<TestItem>()).Returns(itemOptions);

            _cosmosClient.Setup(o =>
                    o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
                .ReturnsAsync(_databaseResponse.Object);

            _database.Setup(o =>
                    o.CreateContainerIfNotExistsAsync(
                        It.Is<ContainerProperties>(c => ValidateContainerProperties(c)),
                        itemOptions.ThroughputProperties,
                        null,
                        CancellationToken.None))
                .ReturnsAsync(_containerResponse.Object);

            _container.Setup(o => o.Id).Returns("a");

            //Act
            await service.GetContainerAsync<TestItem>();
            Container container = await service.GetContainerAsync<TestItem>();

            //Assert
            _container.Verify(o => o.ReplaceContainerAsync(It.Is<ContainerProperties>(c => ValidateContainerProperties(c)), null, CancellationToken.None), Times.Once);
            _container.Verify(o => o.ReplaceThroughputAsync(itemOptions.ThroughputProperties, null, CancellationToken.None), Times.Once);

            Assert.Equal(_container.Object, container);
        }

        [Fact]
        public async Task GetContainerAsyncWhenSyncContainerPropertiesIsSetAndConatinerHasAlreadyBeenSyncByItemSharingTheContainerDoesNotSyncContainerAgain()
        {
            //Arrange
            ICosmosContainerService service = CreateDefaultCosmosContainerService();
            _repositoryOptions.ContainerPerItemType = true;

            ItemOptions testItemOptions = new (typeof(TestItem), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400),5, true);

            ItemOptions anotherTestItemOptions = new (typeof(AnotherTestItem), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400),5, true);

            _itemConfigurationProvider.Setup(o => o.GetOptions<TestItem>()).Returns(testItemOptions);
            _itemConfigurationProvider.Setup(o => o.GetOptions<AnotherTestItem>()).Returns(anotherTestItemOptions);

            _cosmosClient.Setup(o =>
                    o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
                .ReturnsAsync(_databaseResponse.Object);

            _database.Setup(o =>
                    o.CreateContainerIfNotExistsAsync(
                        It.Is<ContainerProperties>(c => ValidateContainerProperties(c)),
                        It.IsAny<ThroughputProperties>(),
                        null,
                        CancellationToken.None))
                .ReturnsAsync(_containerResponse.Object);

            _container.Setup(o => o.Id).Returns("a");

            //Act
            await service.GetContainerAsync<TestItem>();
            Container container = await service.GetContainerAsync<AnotherTestItem>();

            //Assert
            _container.Verify(o => o.ReplaceContainerAsync(It.Is<ContainerProperties>(c => ValidateContainerProperties(c)), null, CancellationToken.None), Times.Once);
            _container.Verify(o => o.ReplaceThroughputAsync(It.IsAny<ThroughputProperties>(), null, CancellationToken.None), Times.Once);

            Assert.Equal(_container.Object, container);
        }

        static bool ValidateContainerProperties(ContainerProperties properties) =>
            properties.Id == "a"
            && properties.PartitionKeyPath == "/test"
            && properties.DefaultTimeToLive == 5;

    }
}