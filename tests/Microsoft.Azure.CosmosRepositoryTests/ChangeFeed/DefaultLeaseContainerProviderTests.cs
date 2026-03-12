// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed;

public class DefaultLeaseContainerProviderTests
{
    private static ILeaseContainerProvider CreateSut(
        ICosmosClientProvider cosmosClientProvider,
        Func<RepositoryOptions> getCurrentOptions)
    {
        Mock<IOptionsMonitor<RepositoryOptions>> monitor = new();
        monitor.SetupGet(options => options.CurrentValue)
            .Returns(getCurrentOptions);

        return new DefaultLeaseContainerProvider(cosmosClientProvider, monitor.Object);
    }

    [Fact]
    public async Task GetLeaseContainerAsync_WhenOptionsChangeBeforeFirstInvocation_UsesUpdatedOptionsForExistingResources()
    {
        // Arrange
        RepositoryOptions initialOptions = CreateOptions("initial-db", true);
        RepositoryOptions updatedOptions = CreateOptions("updated-db", false);
        Mock<CosmosClient> cosmosClient = new();
        Mock<Database> database = new();
        Mock<Container> container = new();
        RepositoryOptions currentOptions = initialOptions;
        ILeaseContainerProvider sut = CreateSut(
            new TestCosmosClientProvider(cosmosClient.Object),
            () => currentOptions);

        cosmosClient.Setup(client => client.GetDatabase(updatedOptions.DatabaseId))
            .Returns(database.Object);

        database.Setup(value => value.GetContainer("lease"))
            .Returns(container.Object);

        currentOptions = updatedOptions;

        // Act
        Container leaseContainer = await sut.GetLeaseContainerAsync();

        // Assert
        Assert.Equal(container.Object, leaseContainer);
        cosmosClient.Verify(client => client.GetDatabase(updatedOptions.DatabaseId), Times.Once);
        cosmosClient.Verify(client => client.GetDatabase(initialOptions.DatabaseId), Times.Never);
        cosmosClient.Verify(
            client => client.CreateDatabaseIfNotExistsAsync(It.IsAny<string>(), (int?)null, null, CancellationToken.None),
            Times.Never);
        database.Verify(value => value.GetContainer("lease"), Times.Once);
        database.Verify(
            value => value.CreateContainerIfNotExistsAsync(It.IsAny<string>(), It.IsAny<string>(), (int?)null, null, CancellationToken.None),
            Times.Never);
    }

    [Fact]
    public async Task GetLeaseContainerAsync_WhenOptionsChangeBeforeFirstInvocation_UsesUpdatedOptionsForAutoCreatedResources()
    {
        // Arrange
        RepositoryOptions initialOptions = CreateOptions("initial-db", false);
        RepositoryOptions updatedOptions = CreateOptions("updated-db", true);
        Mock<CosmosClient> cosmosClient = new();
        Mock<Database> database = new();
        Mock<Container> container = new();
        Mock<DatabaseResponse> databaseResponse = new();
        Mock<ContainerResponse> containerResponse = new();
        RepositoryOptions currentOptions = initialOptions;
        ILeaseContainerProvider sut = CreateSut(
            new TestCosmosClientProvider(cosmosClient.Object),
            () => currentOptions);

        databaseResponse.Setup(value => value.Database)
            .Returns(database.Object);

        containerResponse.Setup(value => value.Container)
            .Returns(container.Object);

        cosmosClient.Setup(client =>
                client.CreateDatabaseIfNotExistsAsync(updatedOptions.DatabaseId, (int?)null, null, CancellationToken.None))
            .ReturnsAsync(databaseResponse.Object);

        database.Setup(value =>
                value.CreateContainerIfNotExistsAsync("lease", "/id", (int?)null, null, CancellationToken.None))
            .ReturnsAsync(containerResponse.Object);

        currentOptions = updatedOptions;

        // Act
        Container leaseContainer = await sut.GetLeaseContainerAsync();

        // Assert
        Assert.Equal(container.Object, leaseContainer);
        cosmosClient.Verify(client => client.GetDatabase(It.IsAny<string>()), Times.Never);
        cosmosClient.Verify(
            client => client.CreateDatabaseIfNotExistsAsync(updatedOptions.DatabaseId, (int?)null, null, CancellationToken.None),
            Times.Once);
        cosmosClient.Verify(
            client => client.CreateDatabaseIfNotExistsAsync(initialOptions.DatabaseId, (int?)null, null, CancellationToken.None),
            Times.Never);
        database.Verify(value => value.GetContainer(It.IsAny<string>()), Times.Never);
        database.Verify(
            value => value.CreateContainerIfNotExistsAsync("lease", "/id", (int?)null, null, CancellationToken.None),
            Times.Once);
    }

    private static RepositoryOptions CreateOptions(string databaseId, bool autoCreate) =>
        new()
        {
            DatabaseId = databaseId,
            IsAutoResourceCreationIfNotExistsEnabled = autoCreate
        };
}
