// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class TransactionalBatchTests(ITestOutputHelper testOutputHelper) : CosmosRepositoryAcceptanceTest(testOutputHelper, DefaultTestRepositoryOptions)
{
    [Fact(Skip = "In discussing this with Bill, we've decided that this might not be reliable enough to justify having it be a release gate.")]
    public async Task BatchRepository_Items_BehavesCorrectlty()
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);

            StockInformation stockInformation = new(5, DateTime.UtcNow);

            IEnumerable<Product> products = Enumerable.Range(0, 5).Select(x => new Product(
                $"Product {x}",
                TechnologyCategoryId,
                500,
                stockInformation)).ToList();

            await _productsRepository.CreateAsBatchAsync(products);

            List<Product> createdProducts =
                await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId).ToListAsync();

            createdProducts.Count.Should().Be(5);
            createdProducts.Should().BeEquivalentTo(products, x =>
                x.Excluding(p => p.Etag)
                    .Excluding(p => p.CreatedTimeUtc)
                    .Excluding(p => p.LastUpdatedTimeRaw)
                    .Excluding(p => p.LastUpdatedTimeUtc));

            foreach (Product product in createdProducts)
            {
                product.Price++;
            }

            await _productsRepository.UpdateAsBatchAsync(createdProducts);

            List<Product> updatedProducts =
                await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId).ToListAsync();

            updatedProducts.Count.Should().Be(5);
            updatedProducts.Should().BeEquivalentTo(createdProducts, x =>
                x.Excluding(p => p.Etag)
                    .Excluding(p => p.CreatedTimeUtc)
                    .Excluding(p => p.LastUpdatedTimeRaw)
                    .Excluding(p => p.LastUpdatedTimeUtc));

            List<Product> productsWithNowOutOfDateEtags = createdProducts;

            await Assert.ThrowsAsync<BatchOperationException<Product>>(() =>
                _productsRepository.UpdateAsBatchAsync(productsWithNowOutOfDateEtags).AsTask());

            await _productsRepository.DeleteAsBatchAsync(updatedProducts);
            var count = await _productsRepository.CountAsync(x => x.PartitionKey == TechnologyCategoryId);
            count.Should().Be(0);
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }
}