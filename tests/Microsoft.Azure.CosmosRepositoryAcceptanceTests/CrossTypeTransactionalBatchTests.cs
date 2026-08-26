namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class CrossTypeTransactionalBatchTests(ITestOutputHelper testOutputHelper)
    : CosmosRepositoryAcceptanceTest(testOutputHelper, DefaultTestRepositoryOptions)
{

    [Fact(Skip = "This might not be reliable enough to justify having it be a release gate.")]
    public Task Batch_MixedProductAndRating_CommitsAtomically() =>
        WithPrunedDatabaseAsync(async () =>
        {
            const string sharedPartitionKey = TechnologyCategoryId;

            Product product = CreateProduct("Widget", sharedPartitionKey, 9.99);
            Rating rating = CreateRating(sharedPartitionKey, 5, "great");

            await _provider.GetRequiredService<IBatchBuilderFactory>()
                .CreateBatch(sharedPartitionKey)
                .CreateItem(product)
                .CreateItem(rating)
                .ExecuteAsync();

            Product storedProduct = await _productsRepository.GetAsync(product.Id, sharedPartitionKey);
            Rating storedRating = await _ratingsRepository.GetAsync(rating.Id, sharedPartitionKey);

            storedProduct.Should().BeEquivalentTo(product, DefaultProductEquivalencyOptions);
            storedRating.Should().BeEquivalentTo(rating, DefaultRatingEquivalencyOptions);
        });

    [Fact(Skip = "This might not be reliable enough to justify having it be a release gate.")]
    public Task Batch_MixedReplaceUpsertDelete_AppliesExpectedState() =>
        WithPrunedDatabaseAsync(async () =>
        {
            const string sharedPartitionKey = TechnologyCategoryId;

            Product existingProduct = await _productsRepository.CreateAsync(
                CreateProduct("Widget", sharedPartitionKey, 9.99));
            Rating existingRating = await _ratingsRepository.CreateAsync(
                CreateRating(sharedPartitionKey, 4, "original"));
            Rating obsoleteRating = await _ratingsRepository.CreateAsync(
                CreateRating(sharedPartitionKey, 1, "obsolete"));

            Product replacementProduct = await _productsRepository.GetAsync(existingProduct.Id, sharedPartitionKey);
            replacementProduct.Price = 12.50;

            Rating upsertedRating = CreateRating(sharedPartitionKey, 5, "updated");
            upsertedRating.Id = existingRating.Id;

            await _provider.GetRequiredService<IBatchBuilderFactory>()
                .CreateBatch(sharedPartitionKey)
                .ReplaceItem(replacementProduct)
                .UpsertItem(upsertedRating)
                .DeleteItem<Rating>(obsoleteRating.Id)
                .ExecuteAsync();

            Product storedProduct = await _productsRepository.GetAsync(existingProduct.Id, sharedPartitionKey);
            Rating storedRating = await _ratingsRepository.GetAsync(existingRating.Id, sharedPartitionKey);
            Rating? deletedRating = await _ratingsRepository.TryGetAsync(obsoleteRating.Id, sharedPartitionKey);

            storedProduct.Price.Should().Be(12.50);
            storedRating.Stars.Should().Be(5);
            storedRating.Text.Should().Be("updated");
            deletedRating.Should().BeNull();
        });

    [Fact(Skip = "This might not be reliable enough to justify having it be a release gate.")]
    public Task Batch_ConflictFailure_RollsBackEarlierOperations() =>
        WithPrunedDatabaseAsync(async () =>
        {
            const string sharedPartitionKey = TechnologyCategoryId;

            Product existingProduct = await _productsRepository.CreateAsync(
                CreateProduct("Existing", sharedPartitionKey, 50.0));

            Rating ratingThatShouldRollback = CreateRating(sharedPartitionKey, 2, "should-roll-back");
            Product conflictingProduct = CreateProduct("Conflict", sharedPartitionKey, 99.0);
            conflictingProduct.Id = existingProduct.Id;

            BatchOperationException exception = await Assert.ThrowsAsync<BatchOperationException>(() =>
                _provider.GetRequiredService<IBatchBuilderFactory>()
                    .CreateBatch(sharedPartitionKey)
                    .CreateItem(ratingThatShouldRollback)
                    .CreateItem(conflictingProduct)
                    .ExecuteAsync()
                    .AsTask());

            exception.Response.Any(operation => operation.StatusCode == HttpStatusCode.Conflict)
                .Should().BeTrue();

            Rating? rolledBackRating = await _ratingsRepository.TryGetAsync(ratingThatShouldRollback.Id, sharedPartitionKey);
            rolledBackRating.Should().BeNull();

            Product storedProduct = await _productsRepository.GetAsync(existingProduct.Id, sharedPartitionKey);
            storedProduct.Name.Should().Be("Existing");
            storedProduct.Price.Should().Be(50.0);
        });

    [Fact(Skip = "This might not be reliable enough to justify having it be a release gate.")]
    public Task Batch_StaleEtagFailure_RollsBackEarlierOperations() =>
        WithPrunedDatabaseAsync(async () =>
        {
            const string sharedPartitionKey = TechnologyCategoryId;

            Product createdProduct = await _productsRepository.CreateAsync(
                CreateProduct("Widget", sharedPartitionKey, 9.99));
            Product staleProduct = await _productsRepository.GetAsync(createdProduct.Id, sharedPartitionKey);

            Product currentProduct = await _productsRepository.GetAsync(createdProduct.Id, sharedPartitionKey);
            currentProduct.Price = 15.99;
            await _productsRepository.UpdateAsync(currentProduct);

            staleProduct.Price = 11.99;

            Rating ratingThatShouldRollback = CreateRating(sharedPartitionKey, 3, "etag-roll-back");

            BatchOperationException exception = await Assert.ThrowsAsync<BatchOperationException>(() =>
                _provider.GetRequiredService<IBatchBuilderFactory>()
                    .CreateBatch(sharedPartitionKey)
                    .CreateItem(ratingThatShouldRollback)
                    .ReplaceItem(staleProduct)
                    .ExecuteAsync()
                    .AsTask());

            exception.Response.Any(operation => operation.StatusCode == HttpStatusCode.PreconditionFailed)
                .Should().BeTrue();

            Rating? rolledBackRating = await _ratingsRepository.TryGetAsync(ratingThatShouldRollback.Id, sharedPartitionKey);
            rolledBackRating.Should().BeNull();

            Product storedProduct = await _productsRepository.GetAsync(staleProduct.Id, sharedPartitionKey);
            storedProduct.Price.Should().Be(15.99);
        });

    private async Task WithPrunedDatabaseAsync(Func<Task> test)
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);
            await test();
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    private static Product CreateProduct(string name, string partitionKey, double price) =>
        new(name, partitionKey, price, new StockInformation(3, DateTime.UtcNow));

    private static Rating CreateRating(string partitionKey, int stars, string text) =>
        new(productId: partitionKey, stars: stars, text: text, categoryId: partitionKey);
}

/// <summary>
/// Exercises cross-type batches with the default ContainerPerItemType = false,
/// where differently named item types share the single physical container.
/// </summary>
[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class CrossTypeTransactionalBatchDefaultOptionsTests(ITestOutputHelper testOutputHelper)
    : CosmosRepositoryAcceptanceTest(testOutputHelper, SharedContainerOptions)
{
    private static readonly Action<RepositoryOptions> SharedContainerOptions = options =>
    {
        options.CosmosConnectionString = GetCosmosConnectionString();
        options.DatabaseId = BuildDatabaseName("products");
        options.ContainerId = "cross-type-batches";
        options.ContainerBuilder.Configure<Product>(builder => builder.WithPartitionKey(DefaultPartitionKey));
        options.ContainerBuilder.Configure<Rating>(builder => builder.WithPartitionKey(DefaultPartitionKey));
    };

    [Fact]
    public async Task Batch_MixedProductAndRating_WithSharedDefaultContainer_CommitsAtomically()
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);

            const string sharedPartitionKey = TechnologyCategoryId;

            Product product = new("Widget", sharedPartitionKey, 9.99, new StockInformation(3, DateTime.UtcNow));
            Rating rating = new(productId: sharedPartitionKey, stars: 5, text: "great", categoryId: sharedPartitionKey);

            await _provider.GetRequiredService<IBatchBuilderFactory>()
                .CreateBatch(sharedPartitionKey)
                .CreateItem(product)
                .CreateItem(rating)
                .ExecuteAsync();

            Product storedProduct = await _productsRepository.GetAsync(product.Id, sharedPartitionKey);
            Rating storedRating = await _ratingsRepository.GetAsync(rating.Id, sharedPartitionKey);

            storedProduct.Should().BeEquivalentTo(product, DefaultProductEquivalencyOptions);
            storedRating.Should().BeEquivalentTo(rating, DefaultRatingEquivalencyOptions);
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }
}

/// <summary>
/// Exercises cross-type batch serialization through a custom Cosmos serializer.
/// </summary>
[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class CrossTypeTransactionalBatchCustomSerializerTests : CosmosRepositoryAcceptanceTest
{
    private readonly RecordingCosmosSerializer _serializer;

    public CrossTypeTransactionalBatchCustomSerializerTests(ITestOutputHelper testOutputHelper)
        : this(testOutputHelper, new RecordingCosmosSerializer())
    {
    }

    private CrossTypeTransactionalBatchCustomSerializerTests(
        ITestOutputHelper testOutputHelper,
        RecordingCosmosSerializer serializer)
        : base(
            testOutputHelper,
            SharedContainerOptions,
            clientOptions =>
            {
                clientOptions.SerializerOptions = null;
                clientOptions.Serializer = serializer;
            })
    {
        _serializer = serializer;
    }

    private static readonly Action<RepositoryOptions> SharedContainerOptions = options =>
    {
        options.CosmosConnectionString = GetCosmosConnectionString();
        options.DatabaseId = BuildDatabaseName("products");
        options.ContainerId = "cross-type-batches-custom-serializer";
        options.ContainerBuilder.Configure<Product>(builder => builder.WithPartitionKey(DefaultPartitionKey));
        options.ContainerBuilder.Configure<Rating>(builder => builder.WithPartitionKey(DefaultPartitionKey));
    };

    [Fact]
    public async Task Batch_MixedPayloadOperations_PreserveConcreteTypesAtSerializerBoundary()
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);

            const string sharedPartitionKey = TechnologyCategoryId;

            Product replacementProduct = await _productsRepository.CreateAsync(
                new Product(
                    "Existing",
                    sharedPartitionKey,
                    9.99,
                    new StockInformation(3, DateTime.UtcNow)));
            replacementProduct.Price = 12.50;

            Rating createdRating = new(
                productId: sharedPartitionKey,
                stars: 5,
                text: "created",
                categoryId: sharedPartitionKey);
            Rating upsertedRating = new(
                productId: sharedPartitionKey,
                stars: 4,
                text: "upserted",
                categoryId: sharedPartitionKey);

            _serializer.ClearRecordedItemTypes();

            await _provider.GetRequiredService<IBatchBuilderFactory>()
                .CreateBatch(sharedPartitionKey)
                .CreateItem(createdRating)
                .ReplaceItem(replacementProduct)
                .UpsertItem(upsertedRating)
                .ExecuteAsync();

            _serializer.SerializedItemTypes.Should().BeEquivalentTo(
                [typeof(Rating), typeof(Product), typeof(Rating)]);
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    private sealed class RecordingCosmosSerializer : CosmosSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };

        private readonly System.Collections.Concurrent.ConcurrentQueue<Type> _serializedItemTypes = new();

        internal IReadOnlyCollection<Type> SerializedItemTypes => _serializedItemTypes;

        internal void ClearRecordedItemTypes() => _serializedItemTypes.Clear();

        public override T FromStream<T>(Stream stream)
        {
            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            using (stream)
            using (StreamReader streamReader = new(stream))
            {
                return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd(), SerializerSettings)!;
            }
        }

        public override Stream ToStream<T>(T input)
        {
            if (input is IItem)
            {
                _serializedItemTypes.Enqueue(typeof(T));

                if (typeof(T) == typeof(object))
                {
                    throw new InvalidOperationException(
                        "A transactional batch item was serialized as object instead of its concrete generic type.");
                }
            }

            byte[] payload = System.Text.Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(input, SerializerSettings));
            return new MemoryStream(payload);
        }
    }
}
