namespace Microsoft.Azure.CosmosRepositoryTests.Batching;

public class InMemoryBatchBuilderTests : IDisposable
{
    public InMemoryBatchBuilderTests() => ClearStorage();

    public void Dispose() => ClearStorage();

    private static IBatchBuilder CreateBuilder(string partitionKey) =>
        new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .BuildServiceProvider()
            .GetRequiredService<IBatchBuilderFactory>()
            .CreateBatch(partitionKey);

    [Fact]
    public async Task Batch_NoOps_Throws()
    {
        // Arrange
        IBatchBuilder builder = CreateBuilder("shared");

        // Act
        Func<Task> act = () => builder.ExecuteAsync().AsTask();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public void Batch_PartitionKeyMismatch_Throws()
    {
        // Arrange
        IBatchBuilder builder = CreateBuilder("B");

        // Act
        Action act = () => builder.CreateItem(new BatchSeedItem { Id = "A" });

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public async Task Batch_MixedOperations_AppliesSequentially()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        InMemoryRepository<BatchSeedItem> testItems = new();
        InMemoryRepository<BatchDeleteItem> otherItems = new();
        InMemoryRepository<BatchCreateItem> createdItems = new();

        BatchSeedItem existing = await testItems.CreateAsync(new BatchSeedItem
        {
            Id = sharedPartitionKey,
            Property = "before"
        });

        await otherItems.CreateAsync(new BatchDeleteItem
        {
            Id = sharedPartitionKey,
            Property = "delete-me"
        });

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .CreateItem(new BatchCreateItem { Id = sharedPartitionKey })
            .UpsertItem(new BatchSeedItem(existing.Etag!)
            {
                Id = sharedPartitionKey,
                Property = "after"
            })
            .DeleteItem<BatchDeleteItem>(sharedPartitionKey);

        // Act
        await builder.ExecuteAsync();

        // Assert
        BatchSeedItem updated = await testItems.GetAsync(sharedPartitionKey);
        updated.Property.Should().Be("after");

        BatchCreateItem created = await createdItems.GetAsync(sharedPartitionKey);
        created.Id.Should().Be(sharedPartitionKey);

        CosmosException deleteException = await Assert.ThrowsAsync<CosmosException>(() =>
            otherItems.GetAsync(sharedPartitionKey).AsTask());
        deleteException.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Batch_CreateConflict_ThrowsCosmosException()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        InMemoryRepository<BatchSeedItem> testItems = new();
        InMemoryRepository<BatchDeleteItem> createdBeforeConflictItems = new();

        await testItems.CreateAsync(new BatchSeedItem
        {
            Id = sharedPartitionKey,
            Property = "existing"
        });

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .CreateItem(new BatchDeleteItem
            {
                Id = sharedPartitionKey,
                Property = "created-first"
            })
            .CreateItem(new BatchSeedItem
            {
                Id = sharedPartitionKey,
                Property = "duplicate"
            });

        // Act
        CosmosException exception = await Assert.ThrowsAsync<CosmosException>(() => builder.ExecuteAsync().AsTask());

        // Assert
        exception.StatusCode.Should().Be(HttpStatusCode.Conflict);

        BatchDeleteItem createdBeforeConflict = await createdBeforeConflictItems.GetAsync(sharedPartitionKey);
        createdBeforeConflict.Property.Should().Be("created-first");
    }

    [Fact]
    public async Task Batch_EtagMismatch_ThrowsCosmosException()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        InMemoryRepository<BatchSeedItem> repository = new();

        await repository.CreateAsync(new BatchSeedItem
        {
            Id = sharedPartitionKey,
            Property = "current"
        });

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .UpsertItem(new BatchSeedItem("stale-etag")
            {
                Id = sharedPartitionKey,
                Property = "updated"
            });

        // Act
        CosmosException exception = await Assert.ThrowsAsync<CosmosException>(() => builder.ExecuteAsync().AsTask());

        // Assert
        exception.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);

        BatchSeedItem stored = await repository.GetAsync(sharedPartitionKey);
        stored.Property.Should().Be("current");
    }

    [Fact]
    public void Batch_AtMaxItems_DoesNotThrow()
    {
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey);

        for (int i = 0; i < BatchConstants.MaxBatchSize; i++)
        {
            builder.CreateItem(new BatchCreateItem { Id = sharedPartitionKey });
        }
    }

    [Fact]
    public void Batch_ExceedsMaxItems_ThrowsInvalidOperationException()
    {
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey);

        for (int i = 0; i < BatchConstants.MaxBatchSize; i++)
        {
            builder.CreateItem(new BatchCreateItem { Id = sharedPartitionKey });
        }

        Action act = () => builder.CreateItem(new BatchCreateItem { Id = sharedPartitionKey });

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
            builder.DeleteItem<BatchCreateItem>(sharedPartitionKey);
        }

        Action act = () => builder.DeleteItem<BatchCreateItem>(sharedPartitionKey);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public async Task Batch_ExecuteAsync_PublishesChangesToInMemoryChangeFeed()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        RecordingChangeFeedProcessor processor = new();

        IServiceProvider provider = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .AddSingleton<IItemChangeFeedProcessor<BatchCreateItem>>(processor)
            .BuildServiceProvider();

        provider.GetRequiredService<InMemoryChangeFeed<BatchCreateItem>>().Setup();

        IBatchBuilder builder = provider.GetRequiredService<IBatchBuilderFactory>()
            .CreateBatch(sharedPartitionKey)
            .CreateItem(new BatchCreateItem { Id = sharedPartitionKey });

        // Act
        await builder.ExecuteAsync();

        // Assert
        processor.ReceivedItems.Should().ContainSingle(item => item.Id == sharedPartitionKey);
    }

    private sealed class RecordingChangeFeedProcessor : IItemChangeFeedProcessor<BatchCreateItem>
    {
        public List<BatchCreateItem> ReceivedItems { get; } = [];

        public ValueTask HandleAsync(BatchCreateItem item, CancellationToken cancellationToken)
        {
            ReceivedItems.Add(item);
            return ValueTask.CompletedTask;
        }
    }

    private static void ClearStorage()
    {
        InMemoryStorage.GetDictionary<BatchSeedItem>().Clear();
        InMemoryStorage.GetDictionary<BatchDeleteItem>().Clear();
        InMemoryStorage.GetDictionary<BatchCreateItem>().Clear();
    }

    private sealed class BatchSeedItem : FullItem
    {
        public BatchSeedItem()
        {
        }

        public BatchSeedItem(string etag) : base(etag)
        {
        }

        public string Property { get; set; } = default!;
    }

    private sealed class BatchDeleteItem : FullItem
    {
        public string Property { get; set; } = default!;
    }

    private sealed class BatchCreateItem : Item
    {
        public string Property { get; set; } = default!;
    }
}
