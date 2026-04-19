namespace Microsoft.Azure.CosmosRepositoryTests.Batching;

public class InMemoryBatchBuilderTests : IDisposable
{
    public InMemoryBatchBuilderTests() => ClearStorage();

    public void Dispose() => ClearStorage();

    [Fact]
    public async Task Batch_NoOps_Throws()
    {
        // Arrange
        IBatchBuilder builder = new InMemoryBatchBuilder("shared");

        // Act
        Func<Task> act = () => builder.ExecuteAsync().AsTask();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public void Batch_PartitionKeyMismatch_Throws()
    {
        // Arrange
        IBatchBuilder builder = new InMemoryBatchBuilder("B");

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

        IBatchBuilder builder = new InMemoryBatchBuilder(sharedPartitionKey)
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

        IBatchBuilder builder = new InMemoryBatchBuilder(sharedPartitionKey)
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

        IBatchBuilder builder = new InMemoryBatchBuilder(sharedPartitionKey)
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
