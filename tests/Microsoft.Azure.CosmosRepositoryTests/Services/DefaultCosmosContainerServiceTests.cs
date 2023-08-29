// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Services;

public class DefaultCosmosContainerServiceTests
{
    readonly Mock<IOptions<RepositoryOptions>> _options = new();
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

        ItemConfiguration itemConfiguration = new(
            typeof(TestItemWithEtag),
            "a",
            "/id",
            new(),
            ThroughputProperties.CreateManualThroughput(400));

        _itemConfigurationProvider.Setup(o => o.GetItemConfiguration(typeof(TestItemWithEtag))).Returns(itemConfiguration);

        _cosmosClient.Setup(o =>
                o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
            .ReturnsAsync(_databaseResponse.Object);

        _database.Setup(o =>
            o.CreateContainerIfNotExistsAsync(
                It.Is<ContainerProperties>(c => c.Id == "containerA"
                                                && c.PartitionKeyPath == "/id"),
                itemConfiguration.ThroughputProperties,
                null,
                CancellationToken.None))
            .ReturnsAsync(_containerResponse.Object);

        //Act
        Container container = await service.GetContainerAsync<TestItemWithEtag>();

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

        ItemConfiguration itemConfiguration = new(typeof(TestItemWithEtag), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400), 5);

        _itemConfigurationProvider.Setup(o => o.GetItemConfiguration(typeof(TestItemWithEtag))).Returns(itemConfiguration);

        _cosmosClient.Setup(o =>
                o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
            .ReturnsAsync(_databaseResponse.Object);

        _database.Setup(o =>
                o.CreateContainerIfNotExistsAsync(
                    It.Is<ContainerProperties>(c => c.Id == "a"
                                                    && c.PartitionKeyPath == "/test"
                                                    && c.DefaultTimeToLive == 5),
                    itemConfiguration.ThroughputProperties,
                    null,
                    CancellationToken.None))
            .ReturnsAsync(_containerResponse.Object);

        //Act
        Container container = await service.GetContainerAsync<TestItemWithEtag>();

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

        ItemConfiguration itemConfiguration = new(typeof(TestItemWithEtag), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400), 5, true);

        _itemConfigurationProvider.Setup(o => o.GetItemConfiguration(typeof(TestItemWithEtag))).Returns(itemConfiguration);

        _cosmosClient.Setup(o =>
                o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
            .ReturnsAsync(_databaseResponse.Object);

        _database.Setup(o =>
                o.CreateContainerIfNotExistsAsync(
                    It.Is<ContainerProperties>(c => ValidateContainerProperties(c)),
                    itemConfiguration.ThroughputProperties,
                    null,
                    CancellationToken.None))
            .ReturnsAsync(_containerResponse.Object);

        _container.Setup(o => o.Id).Returns("a");

        //Act
        Container container = await service.GetContainerAsync<TestItemWithEtag>();

        //Assert
        _container.Verify(o => o.ReplaceContainerAsync(It.Is<ContainerProperties>(c => ValidateContainerProperties(c)), null, CancellationToken.None), Times.Once);
        _container.Verify(o => o.ReplaceThroughputAsync(itemConfiguration.ThroughputProperties, null, CancellationToken.None), Times.Once);

        Assert.Equal(_container.Object, container);
    }

    [Fact]
    public async Task GetContainerAsyncWhenSyncContainerPropertiesIsFalseButForceIsSetContainerProperties()
    {
        //Arrange
        ICosmosContainerService service = CreateDefaultCosmosContainerService();
        _repositoryOptions.ContainerPerItemType = true;

        ItemConfiguration itemConfiguration = new(typeof(TestItemWithEtag), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400), 5, false);

        _itemConfigurationProvider.Setup(o => o.GetItemConfiguration(typeof(TestItemWithEtag))).Returns(itemConfiguration);

        _cosmosClient.Setup(o =>
                o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
            .ReturnsAsync(_databaseResponse.Object);

        _database.Setup(o =>
                o.CreateContainerIfNotExistsAsync(
                    It.Is<ContainerProperties>(c => ValidateContainerProperties(c)),
                    itemConfiguration.ThroughputProperties,
                    null,
                    CancellationToken.None))
            .ReturnsAsync(_containerResponse.Object);

        //Act
        Container container = await service.GetContainerAsync<TestItemWithEtag>(true);

        //Assert
        _container.Verify(o => o.ReplaceContainerAsync(It.Is<ContainerProperties>(c => ValidateContainerProperties(c)), null, CancellationToken.None), Times.Once);
        _container.Verify(o => o.ReplaceThroughputAsync(itemConfiguration.ThroughputProperties, null, CancellationToken.None), Times.Once);

        Assert.Equal(_container.Object, container);
    }

    [Fact]
    public async Task GetContainerAsyncWhenSyncContainerPropertiesIsSetAndConatinerHasAlreadyBeenSyncDoesNotSyncContainerAgain()
    {
        //Arrange
        ICosmosContainerService service = CreateDefaultCosmosContainerService();
        _repositoryOptions.ContainerPerItemType = true;

        ItemConfiguration itemConfiguration = new(typeof(TestItemWithEtag), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400), 5, true);

        _itemConfigurationProvider.Setup(o => o.GetItemConfiguration(typeof(TestItemWithEtag))).Returns(itemConfiguration);

        _cosmosClient.Setup(o =>
                o.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId, (int?)null, null, CancellationToken.None))
            .ReturnsAsync(_databaseResponse.Object);

        _database.Setup(o =>
                o.CreateContainerIfNotExistsAsync(
                    It.Is<ContainerProperties>(c => ValidateContainerProperties(c)),
                    itemConfiguration.ThroughputProperties,
                    null,
                    CancellationToken.None))
            .ReturnsAsync(_containerResponse.Object);

        _container.Setup(o => o.Id).Returns("a");

        //Act
        await service.GetContainerAsync<TestItemWithEtag>();
        Container container = await service.GetContainerAsync<TestItemWithEtag>();

        //Assert
        _container.Verify(o => o.ReplaceContainerAsync(It.Is<ContainerProperties>(c => ValidateContainerProperties(c)), null, CancellationToken.None), Times.Once);
        _container.Verify(o => o.ReplaceThroughputAsync(itemConfiguration.ThroughputProperties, null, CancellationToken.None), Times.Once);

        Assert.Equal(_container.Object, container);
    }

    [Fact]
    public async Task GetContainerAsyncWhenSyncContainerPropertiesIsSetAndContainerHasAlreadyBeenSyncByItemSharingTheContainerDoesNotSyncContainerAgain()
    {
        //Arrange
        ICosmosContainerService service = CreateDefaultCosmosContainerService();
        _repositoryOptions.ContainerPerItemType = true;

        ItemConfiguration testItemConfiguration = new(typeof(TestItemWithEtag), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400), 5, true);

        ItemConfiguration anotherTestItemConfiguration = new(typeof(AnotherTestItem), "a", "/test", new(), ThroughputProperties.CreateManualThroughput(400), 5, true);

        _itemConfigurationProvider.Setup(o => o.GetItemConfiguration(typeof(TestItemWithEtag))).Returns(testItemConfiguration);
        _itemConfigurationProvider.Setup(o => o.GetItemConfiguration(typeof(AnotherTestItem))).Returns(anotherTestItemConfiguration);

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
        await service.GetContainerAsync<TestItemWithEtag>();
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