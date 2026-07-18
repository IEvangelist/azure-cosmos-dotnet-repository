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
    public async Task Batch_CreateConflict_ThrowsBatchOperationExceptionAndCommitsNothing()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        InMemoryRepository<BatchSeedItem> testItems = new();
        InMemoryRepository<BatchDeleteItem> earlierOperationItems = new();

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
        BatchOperationException exception =
            await Assert.ThrowsAsync<BatchOperationException>(() => builder.ExecuteAsync().AsTask());

        // Assert
        exception.StatusCode.Should().Be(HttpStatusCode.Conflict);
        exception.Response.Should().BeNull();
        exception.InnerException.Should().BeOfType<CosmosException>();

        BatchDeleteItem? earlierOperationItem = await earlierOperationItems.TryGetAsync(sharedPartitionKey);
        earlierOperationItem.Should().BeNull();
    }

    [Fact]
    public async Task Batch_CreateWithStoredDiscriminatorMismatch_CommitsNothing()
    {
        const string sharedPartitionKey = "shared";
        InMemoryRepository<BatchSeedItem> repository = new();

        await repository.CreateAsync(new BatchSeedItem
        {
            Id = sharedPartitionKey,
            Type = nameof(BatchDeleteItem),
            Property = "existing"
        });

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .CreateItem(new BatchDeleteItem
            {
                Id = "earlier",
                Partition = sharedPartitionKey,
                Property = "must-not-commit"
            })
            .CreateItem(new BatchSeedItem
            {
                Id = sharedPartitionKey,
                Property = "duplicate"
            });

        BatchOperationException exception =
            await Assert.ThrowsAsync<BatchOperationException>(() => builder.ExecuteAsync().AsTask());

        exception.StatusCode.Should().Be(HttpStatusCode.Conflict);
        exception.Response.Should().BeNull();
        exception.InnerException.Should().BeOfType<CosmosException>();
        (await new InMemoryRepository<BatchDeleteItem>().TryGetAsync("earlier", sharedPartitionKey)).Should().BeNull();
    }

    [Fact]
    public async Task Batch_CrossTypeCreatesWithSameIdentity_ThrowsConflictAndCommitsNothing()
    {
        const string sharedPartitionKey = "shared";
        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .CreateItem(new BatchCreateItem { Id = sharedPartitionKey })
            .CreateItem(new BatchDeleteItem { Id = sharedPartitionKey });

        BatchOperationException exception =
            await Assert.ThrowsAsync<BatchOperationException>(() => builder.ExecuteAsync().AsTask());

        exception.StatusCode.Should().Be(HttpStatusCode.Conflict);
        exception.Response.Should().BeNull();
        exception.InnerException.Should().BeOfType<CosmosException>();
        (await new InMemoryRepository<BatchCreateItem>().TryGetAsync(sharedPartitionKey)).Should().BeNull();
        (await new InMemoryRepository<BatchDeleteItem>().TryGetAsync(sharedPartitionKey)).Should().BeNull();
    }

    [Fact]
    public async Task Batch_PreCancelledToken_ThrowsAndCommitsNothing()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .CreateItem(new BatchCreateItem { Id = sharedPartitionKey });

        // Act
        Func<Task> act = () => builder.ExecuteAsync(cancellationTokenSource.Token).AsTask();

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(act);

        BatchCreateItem? stored = await new InMemoryRepository<BatchCreateItem>().TryGetAsync(sharedPartitionKey);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task Batch_CreateThenReplaceSameItem_Succeeds()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .CreateItem(new BatchSeedItem
            {
                Id = sharedPartitionKey,
                Property = "created"
            })
            .ReplaceItem(new BatchSeedItem
            {
                Id = sharedPartitionKey,
                Property = "replaced"
            });

        // Act
        await builder.ExecuteAsync();

        // Assert
        BatchSeedItem stored = await new InMemoryRepository<BatchSeedItem>().GetAsync(sharedPartitionKey);
        stored.Property.Should().Be("replaced");
    }

    [Fact]
    public async Task Batch_ReplaceMissingItem_ThrowsNotFoundAndDoesNotCreate()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .ReplaceItem(new BatchSeedItem
            {
                Id = sharedPartitionKey,
                Property = "missing"
            });

        // Act
        BatchOperationException exception =
            await Assert.ThrowsAsync<BatchOperationException>(() => builder.ExecuteAsync().AsTask());

        // Assert
        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.Response.Should().BeNull();
        exception.InnerException.Should().BeOfType<CosmosException>();

        BatchSeedItem? stored = await new InMemoryRepository<BatchSeedItem>().TryGetAsync(sharedPartitionKey);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task Batch_EtagMismatch_ThrowsBatchOperationException()
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
        BatchOperationException exception =
            await Assert.ThrowsAsync<BatchOperationException>(() => builder.ExecuteAsync().AsTask());

        // Assert
        exception.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        exception.Response.Should().BeNull();
        exception.InnerException.Should().BeOfType<CosmosException>();

        BatchSeedItem stored = await repository.GetAsync(sharedPartitionKey);
        stored.Property.Should().Be("current");
    }

    [Fact]
    public async Task Batch_SecondWriteWithOriginalEtag_FailsPrecondition()
    {
        const string sharedPartitionKey = "shared";
        InMemoryRepository<BatchSeedItem> repository = new();
        BatchSeedItem stored = await repository.CreateAsync(new BatchSeedItem
        {
            Id = sharedPartitionKey,
            Property = "before"
        });

        IBatchBuilder builder = CreateBuilder(sharedPartitionKey)
            .UpsertItem(new BatchSeedItem(stored.Etag!)
            {
                Id = sharedPartitionKey,
                Property = "first"
            })
            .ReplaceItem(new BatchSeedItem(stored.Etag!)
            {
                Id = sharedPartitionKey,
                Property = "second"
            });

        BatchOperationException exception =
            await Assert.ThrowsAsync<BatchOperationException>(() => builder.ExecuteAsync().AsTask());

        exception.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        (await repository.GetAsync(sharedPartitionKey)).Property.Should().Be("before");
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
    public async Task Batch_ContainerPerItemTypeWithDifferentContainers_ExecuteAsyncThrows()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .Configure<RepositoryOptions>(options => options.ContainerPerItemType = true)
            .BuildServiceProvider()
            .GetRequiredService<IBatchBuilderFactory>()
            .CreateBatch(sharedPartitionKey)
            .CreateItem(new BatchCreateItem { Id = sharedPartitionKey })
            .CreateItem(new BatchSeedItem { Id = sharedPartitionKey });

        // Act
        Func<Task> act = () => builder.ExecuteAsync().AsTask();

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task Batch_ContainerPerItemTypeWithSharedContainer_ExecuteAsyncSucceeds()
    {
        // Arrange
        const string sharedPartitionKey = "shared";

        IBatchBuilder builder = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .Configure<RepositoryOptions>(options =>
            {
                options.ContainerPerItemType = true;
                options.ContainerBuilder.Configure<SharedContainerItemA>(builder => builder.WithContainer("shared-container"));
                options.ContainerBuilder.Configure<SharedContainerItemB>(builder => builder.WithContainer("shared-container"));
            })
            .BuildServiceProvider()
            .GetRequiredService<IBatchBuilderFactory>()
            .CreateBatch(sharedPartitionKey)
            .CreateItem(new SharedContainerItemA { Id = "a", Partition = sharedPartitionKey })
            .CreateItem(new SharedContainerItemB { Id = "b", Partition = sharedPartitionKey });

        // Act
        await builder.ExecuteAsync();

        // Assert
        SharedContainerItemA created = await new InMemoryRepository<SharedContainerItemA>().GetAsync("a", sharedPartitionKey);
        created.Id.Should().Be("a");
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

    [Fact]
    public async Task Batch_CommitFailure_RestoresStorageAndPublishesNothing()
    {
        const string sharedPartitionKey = "shared";
        RecordingChangeFeedProcessor processor = new();
        IServiceProvider provider = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .AddSingleton<IItemChangeFeedProcessor<BatchCreateItem>>(processor)
            .BuildServiceProvider();
        provider.GetRequiredService<InMemoryChangeFeed<BatchCreateItem>>().Setup();

        IBatchBuilder builder = provider.GetRequiredService<IBatchBuilderFactory>()
            .CreateBatch(sharedPartitionKey)
            .CreateItem(new BatchCreateItem { Id = sharedPartitionKey })
            .CreateItem(new FailingBatchItem { Id = "failure", Partition = sharedPartitionKey });

        await Assert.ThrowsAnyAsync<Exception>(() => builder.ExecuteAsync().AsTask());

        (await new InMemoryRepository<BatchCreateItem>().TryGetAsync(sharedPartitionKey)).Should().BeNull();
        processor.ReceivedItems.Should().BeEmpty();
    }

    [Fact]
    public async Task Batch_ThrowingChangeFeedProcessor_LeavesLaterRepositoryPublishing()
    {
        const string sharedPartitionKey = "shared";
        RecordingChangeFeedProcessor processor = new();
        IServiceProvider provider = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .AddSingleton<IItemChangeFeedProcessor<SharedContainerItemA>>(new ThrowingChangeFeedProcessor())
            .AddSingleton<IItemChangeFeedProcessor<BatchCreateItem>>(processor)
            .BuildServiceProvider();
        provider.GetRequiredService<InMemoryChangeFeed<SharedContainerItemA>>().Setup();
        provider.GetRequiredService<InMemoryChangeFeed<BatchCreateItem>>().Setup();

        IBatchBuilder builder = provider.GetRequiredService<IBatchBuilderFactory>()
            .CreateBatch(sharedPartitionKey)
            .CreateItem(new SharedContainerItemA { Id = "a", Partition = sharedPartitionKey })
            .CreateItem(new BatchCreateItem { Id = sharedPartitionKey });

        await Assert.ThrowsAnyAsync<Exception>(() => builder.ExecuteAsync().AsTask());

        // The failed publish must not leave the second repository buffering, or
        // every later change for that item type is swallowed.
        await provider.GetRequiredService<IRepository<BatchCreateItem>>()
            .CreateAsync(new BatchCreateItem { Id = "later" });

        processor.ReceivedItems.Should().ContainSingle(item => item.Id == "later");
    }

    [Fact]
    public async Task Batch_NonInMemoryRepositoryRegistration_ThrowsBeforeCommit()
    {
        Mock<IRepository<DecoratedBatchItem>> repository = new();
        IServiceProvider provider = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .AddSingleton(repository.Object)
            .BuildServiceProvider();

        IBatchBuilder builder = provider.GetRequiredService<IBatchBuilderFactory>()
            .CreateBatch("shared")
            .CreateItem(new DecoratedBatchItem { Id = "shared" });

        InvalidOperationException exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() => builder.ExecuteAsync().AsTask());

        exception.Message.Should().Contain(nameof(InMemoryRepository<DecoratedBatchItem>));
        InMemoryStorage.GetDictionary<DecoratedBatchItem>().Should().BeEmpty();
    }

    private sealed class ThrowingChangeFeedProcessor : IItemChangeFeedProcessor<SharedContainerItemA>
    {
        public ValueTask HandleAsync(SharedContainerItemA item, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("change feed processor failed");
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
        InMemoryStorage.GetDictionary<SharedContainerItemA>().Clear();
        InMemoryStorage.GetDictionary<SharedContainerItemB>().Clear();
        InMemoryStorage.GetDictionary<FailingBatchItem>().Clear();
        InMemoryStorage.GetDictionary<DecoratedBatchItem>().Clear();
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
        public string? Partition { get; set; }

        public string Property { get; set; } = default!;

        protected override string GetPartitionKeyValue() => Partition ?? Id;
    }

    private sealed class BatchCreateItem : Item
    {
        public string Property { get; set; } = default!;
    }

    private sealed class SharedContainerItemA : Item
    {
        public string Partition { get; set; } = default!;

        protected override string GetPartitionKeyValue() => Partition;
    }

    private sealed class SharedContainerItemB : Item
    {
        public string Partition { get; set; } = default!;

        protected override string GetPartitionKeyValue() => Partition;
    }

    private sealed class FailingBatchItem : Item
    {
        public string Partition { get; set; } = default!;

        [JsonProperty]
        public string FailingValue => throw new InvalidOperationException("serialization failed");

        protected override string GetPartitionKeyValue() => Partition;
    }

    public sealed class DecoratedBatchItem : Item
    {
    }
}
