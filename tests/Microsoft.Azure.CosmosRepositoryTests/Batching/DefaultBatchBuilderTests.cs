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

    [Fact]
    public void Batch_AtMaxItems_DoesNotThrow()
    {
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey);

        for (int i = 0; i < BatchConstants.MaxBatchSize; i++)
        {
            builder.CreateItem(new TestItem { Id = sharedPartitionKey });
        }
    }

    [Fact]
    public void Batch_ExceedsMaxItems_ThrowsInvalidOperationException()
    {
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey);

        for (int i = 0; i < BatchConstants.MaxBatchSize; i++)
        {
            builder.CreateItem(new TestItem { Id = sharedPartitionKey });
        }

        Action act = () => builder.CreateItem(new TestItem { Id = sharedPartitionKey });

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
        exception.Message.Should().Contain(BatchConstants.MaxBatchSize.ToString());
    }

    [Fact]
    public void Batch_ExceedsMaxItemsViaDeleteById_ThrowsInvalidOperationException()
    {
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey);

        for (int i = 0; i < BatchConstants.MaxBatchSize; i++)
        {
            builder.DeleteItem<TestItem>(sharedPartitionKey);
        }

        Action act = () => builder.DeleteItem<TestItem>(sharedPartitionKey);

        Assert.Throws<InvalidOperationException>(act);
    }

    private DefaultBatchBuilder CreateBuilder(string partitionKey) =>
        new(
            partitionKey,
            typeof(TestItem),
            _containerService.Object);
}
