// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Batching;

public class DefaultBatchBuilderTests
{
    private readonly Mock<ICosmosContainerService> _containerService = new();

    [Fact]
    public void CreateBatch_NullPartitionKey_Throws()
    {
        IBatchBuilderFactory factory = new DefaultBatchBuilderFactory(_containerService.Object);

        // Act
        Action act = () => factory.CreateBatch(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Batch_EmptyOps_ExecuteAsync_Throws()
    {
        IBatchBuilder builder = CreateBuilder("A");

        // Act
        Func<Task> act = () => builder.ExecuteAsync().AsTask();

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
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
    public async Task Batch_ExecuteAsync_PreservesGenericItemTypeForSdkCalls()
    {
        const string sharedPartitionKey = "shared";

        Mock<Container> container = new();
        Mock<TransactionalBatch> batch = new();
        Mock<TransactionalBatchResponse> response = new();

        container.Setup(c => c.CreateTransactionalBatch(It.IsAny<PartitionKey>()))
            .Returns(batch.Object);
        batch.Setup(b => b.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);
        response.SetupGet(r => r.IsSuccessStatusCode).Returns(true);

        _containerService.Setup(service => service.GetContainerAsync(It.IsAny<IReadOnlyList<Type>>()))
            .ReturnsAsync(container.Object);

        TestItem created = new() { Id = sharedPartitionKey };
        TestItemOther replaced = new() { Id = sharedPartitionKey };
        TestItem upserted = new() { Id = sharedPartitionKey };

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .CreateItem(created)
            .ReplaceItem(replaced)
            .UpsertItem(upserted);

        await builder.ExecuteAsync();

        // The SDK serializer selects metadata by the generic type argument, so the
        // closed generic must be the item's own type - not object.
        batch.Verify(b => b.CreateItem(created, It.IsAny<TransactionalBatchItemRequestOptions>()), Times.Once);
        batch.Verify(b => b.ReplaceItem(replaced.Id, replaced, It.IsAny<TransactionalBatchItemRequestOptions>()), Times.Once);
        batch.Verify(b => b.UpsertItem(upserted, It.IsAny<TransactionalBatchItemRequestOptions>()), Times.Once);
    }

    [Fact]
    public async Task Batch_QueuedOperations_UseQueuedIdsAndEtags()
    {
        Mock<Container> container = new();
        Mock<TransactionalBatch> batch = new();
        Mock<TransactionalBatchResponse> response = new();
        container.Setup(value => value.CreateTransactionalBatch(It.IsAny<PartitionKey>())).Returns(batch.Object);
        batch.Setup(value => value.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
        response.SetupGet(value => value.IsSuccessStatusCode).Returns(true);
        _containerService.Setup(value => value.GetContainerAsync(It.IsAny<IReadOnlyList<Type>>())).ReturnsAsync(container.Object);

        MutableEtagItem replaced = new()
        {
            Id = "queued-replace-id",
            PartitionKey = "partition",
            Etag = "queued-replace-etag"
        };
        MutableEtagItem upserted = new()
        {
            Id = "queued-upsert-id",
            PartitionKey = "partition",
            Etag = "queued-upsert-etag"
        };
        MutableEtagItem deleted = new()
        {
            Id = "queued-delete-id",
            PartitionKey = "partition"
        };

        IBatchBuilder builder = CreateBuilder("partition")
            .ReplaceItem(replaced)
            .UpsertItem(upserted)
            .DeleteItem(deleted);
        replaced.Etag = "execution-replace-etag";
        upserted.Etag = "execution-upsert-etag";
        deleted.Id = "execution-delete-id";

        await builder.ExecuteAsync();

        batch.Verify(value => value.ReplaceItem(
            "queued-replace-id",
            replaced,
            It.Is<TransactionalBatchItemRequestOptions>(options => options.IfMatchEtag == "queued-replace-etag")), Times.Once);
        batch.Verify(value => value.UpsertItem(
            upserted,
            It.Is<TransactionalBatchItemRequestOptions>(options => options.IfMatchEtag == "queued-upsert-etag")), Times.Once);
        batch.Verify(value => value.DeleteItem(
            "queued-delete-id",
            It.IsAny<TransactionalBatchItemRequestOptions>()), Times.Once);
    }

    [Fact]
    public async Task Batch_ReplaceItemIdChangedAfterQueuing_ThrowsAndSendsNothing()
    {
        Mock<Container> container = new();
        Mock<TransactionalBatch> batch = new();
        container.Setup(value => value.CreateTransactionalBatch(It.IsAny<PartitionKey>())).Returns(batch.Object);
        _containerService.Setup(value => value.GetContainerAsync(It.IsAny<IReadOnlyList<Type>>())).ReturnsAsync(container.Object);

        MutableEtagItem replaced = new()
        {
            Id = "queued-replace-id",
            PartitionKey = "partition"
        };

        IBatchBuilder builder = CreateBuilder("partition").ReplaceItem(replaced);
        replaced.Id = "execution-replace-id";

        // A replace routes on the queued id but serializes the item at
        // execution time. Cosmos treats id as immutable and would fail the
        // whole batch, so this has to fail locally instead.
        Func<Task> act = () => builder.ExecuteAsync().AsTask();

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        exception.Message.Should().Contain("queued-replace-id").And.Contain("execution-replace-id");
        batch.Verify(value => value.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Never);
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
            _containerService.Object);

    private sealed class MutableEtagItem : IItemWithEtag
    {
        public string Id { get; set; } = default!;

        public string Type { get; set; } = nameof(MutableEtagItem);

        public string PartitionKey { get; set; } = default!;

        public string? Etag { get; set; }
    }
}
