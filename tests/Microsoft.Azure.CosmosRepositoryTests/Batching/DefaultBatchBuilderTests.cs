namespace Microsoft.Azure.CosmosRepositoryTests.Batching;

public class DefaultBatchBuilderTests
{
    private readonly Mock<ICosmosContainerService> _containerService = new();
    private readonly Mock<ICosmosItemConfigurationProvider> _configurationProvider = new();

    [Fact]
    public async Task Batch_EmptyOps_ExecuteAsync_Throws()
    {
        // Arrange
        SetupContainer(typeof(TestItem), "container-a");
        IBatchBuilder builder = CreateBuilder("A");

        // Act
        Func<Task> act = () => builder.ExecuteAsync().AsTask();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public void Batch_PartitionKeyMismatch_Throws()
    {
        // Arrange
        SetupContainer(typeof(TestItem), "container-a");
        IBatchBuilder builder = CreateBuilder("B");

        // Act
        Action act = () => builder.CreateItem(new TestItem { Id = "A" });

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Batch_CrossContainer_AtAddTime_Throws()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        SetupContainer(typeof(TestItem), "container-a");
        SetupContainer(typeof(TestItemOther), "container-b");

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey);

        // Act
        Action act = () => builder.CreateItem(new TestItemOther { Id = sharedPartitionKey });

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Batch_SameContainerDifferentTypes_AddAcceptsBoth_Succeeds()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        SetupContainer(typeof(TestItem), "container-a");
        SetupContainer(typeof(TestItemOther), "container-a");

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey);

        // Act
        Exception? exception = Record.Exception(() => builder
            .CreateItem(new TestItem { Id = sharedPartitionKey })
            .DeleteItem<TestItemOther>(sharedPartitionKey));

        // Assert
        Assert.Null(exception);
    }

    private DefaultBatchBuilder CreateBuilder(string partitionKey) =>
        new(
            partitionKey,
            typeof(TestItem),
            _containerService.Object,
            _configurationProvider.Object);

    private void SetupContainer(Type type, string containerName)
    {
        ItemConfiguration configuration = new(
            type,
            containerName,
            "/id",
            new UniqueKeyPolicy(),
            ThroughputProperties.CreateManualThroughput(400));

        _configurationProvider.Setup(provider => provider.GetItemConfiguration(type))
            .Returns(configuration);
    }
}
