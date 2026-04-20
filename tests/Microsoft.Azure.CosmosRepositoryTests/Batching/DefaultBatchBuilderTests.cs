namespace Microsoft.Azure.CosmosRepositoryTests.Batching;

public class DefaultBatchBuilderTests
{
    private readonly Mock<ICosmosContainerService> _containerService = new();

    [Fact]
    public async Task Batch_EmptyOps_ExecuteAsync_Throws()
    {
        IBatchBuilder builder = CreateBuilder("A");

        // Act
        Func<Task> act = () => builder.ExecuteAsync().AsTask();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public void Batch_PartitionKeyMismatch_Throws()
    {
        IBatchBuilder builder = CreateBuilder("B");

        // Act
        Action act = () => builder.CreateItem(new TestItem { Id = "A" });

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Batch_SameContainerDifferentTypes_AddAcceptsBoth_Succeeds()
    {
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey);

        // Act
        var sut = builder
            .CreateItem(new TestItem { Id = sharedPartitionKey })
            .DeleteItem<TestItemOther>(sharedPartitionKey);

        // Assert
        Assert.NotNull(sut);
    }

    [Fact]
    public async Task Batch_ExecuteAsync_PassesTrackedTypesToContainerService()
    {
        const string sharedPartitionKey = "shared";

        Mock<Container> container = new();
        Mock<TransactionalBatch> batch = new();
        Mock<TransactionalBatchResponse> response = new();

        container.Setup(c => c.CreateTransactionalBatch(It.Is<PartitionKey>(key => key == new PartitionKey(sharedPartitionKey))))
            .Returns(batch.Object);
        batch.Setup(b => b.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);
        response.SetupGet(r => r.IsSuccessStatusCode).Returns(true);

        _containerService.Setup(service => service.GetContainerAsync(
                It.Is<IReadOnlyList<Type>>(types =>
                    types.Count == 2 &&
                    types.Contains(typeof(TestItem)) &&
                    types.Contains(typeof(TestItemOther)))))
            .ReturnsAsync(container.Object);

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .CreateItem(new TestItem { Id = sharedPartitionKey })
            .DeleteItem<TestItemOther>(sharedPartitionKey);

        await builder.ExecuteAsync();

        _containerService.Verify(service => service.GetContainerAsync(
            It.Is<IReadOnlyList<Type>>(types =>
                types.Count == 2 &&
                types.Contains(typeof(TestItem)) &&
                types.Contains(typeof(TestItemOther)))), Times.Once);
    }

    private DefaultBatchBuilder CreateBuilder(string partitionKey) =>
        new(
            partitionKey,
            typeof(TestItem),
            _containerService.Object);
}
