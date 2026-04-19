namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class CrossTypeTransactionalBatchTests(ITestOutputHelper testOutputHelper)
    : CosmosRepositoryAcceptanceTest(testOutputHelper)
{

    [Fact]
    public Task Batch_MixedProductAndRating_CommitsAtomically() =>
        WithPrunedDatabaseAsync(async () =>
        {
            const string sharedPartitionKey = TechnologyCategoryId;

            Product product = CreateProduct("Widget", sharedPartitionKey, 9.99);
            Rating rating = CreateRating(sharedPartitionKey, 5, "great");

            await _productsRepository
                .Batch(sharedPartitionKey)
                .CreateItem(product)
                .CreateItem(rating)
                .ExecuteAsync();

            Product storedProduct = await _productsRepository.GetAsync(product.Id, sharedPartitionKey);
            Rating storedRating = await _ratingsRepository.GetAsync(rating.Id, sharedPartitionKey);

            storedProduct.Should().BeEquivalentTo(product, DefaultProductEquivalencyOptions);
            storedRating.Should().BeEquivalentTo(rating, DefaultRatingEquivalencyOptions);
        });

    [Fact]
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

            await _productsRepository
                .Batch(sharedPartitionKey)
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

    [Fact]
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
                _productsRepository
                    .Batch(sharedPartitionKey)
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

    [Fact]
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
                _productsRepository
                    .Batch(sharedPartitionKey)
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
