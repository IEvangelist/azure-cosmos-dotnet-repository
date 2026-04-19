namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class CrossTypeTransactionalBatchTests(ITestOutputHelper testOutputHelper)
    : CosmosRepositoryAcceptanceTest(testOutputHelper, DefaultTestRepositoryOptions)
{
    [Fact(Skip = "This might not be reliable enough to justify having it be a release gate.")]
    public async Task Batch_MixedProductAndRating_CommitsAtomically()
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);

            const string sharedPartitionKey = TechnologyCategoryId;

            Product product = new(
                "Widget",
                sharedPartitionKey,
                9.99,
                new StockInformation(3, DateTime.UtcNow));
            Rating rating = new(
                productId: sharedPartitionKey,
                stars: 5,
                text: "great",
                categoryId: sharedPartitionKey);

            await _productsRepository
                .Batch(sharedPartitionKey)
                .CreateItem(product)
                .CreateItem(rating)
                .ExecuteAsync();

            Product storedProduct = await _productsRepository.GetAsync(product.Id, sharedPartitionKey);
            Rating storedRating = await _ratingsRepository.GetAsync(rating.Id, sharedPartitionKey);

            storedProduct.Should().BeEquivalentTo(product, DefaultProductEquivalencyOptions);
            storedRating.Should().BeEquivalentTo(rating, options => options
                .Excluding(x => x.Etag)
                .Excluding(x => x.CreatedTimeUtc)
                .Excluding(x => x.LastUpdatedTimeRaw)
                .Excluding(x => x.LastUpdatedTimeUtc));
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    [Fact(Skip = "This might not be reliable enough to justify having it be a release gate.")]
    public async Task Batch_CrossContainerItem_Throws()
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);

            const string sharedPartitionKey = TechnologyCategoryId;
            Offer offer = new(sharedPartitionKey, "test-user");

            Action act = () => _productsRepository
                .Batch(sharedPartitionKey)
                .CreateItem(offer);

            act.Should().Throw<InvalidOperationException>();
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }
}
