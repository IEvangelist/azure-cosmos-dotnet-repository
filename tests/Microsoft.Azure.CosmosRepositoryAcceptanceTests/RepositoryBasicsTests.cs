// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class RepositoryBasicsTests(ITestOutputHelper testOutputHelper) : CosmosRepositoryAcceptanceTest(testOutputHelper, DefaultTestRepositoryOptions)
{
    [Fact(Skip = "In discussing this with Bill, we've decided that this might not be reliable enough to justify having it be a release gate.")]
    public async Task ProductRepository_BasicCRUDOperations_WorksCorrectly()
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);

            StockInformation stockInformation = new(5, DateTime.UtcNow);

            Product product = new(
                "Samsung TV",
                TechnologyCategoryId,
                500,
                stockInformation);

            await _productsRepository.CreateAsync(product);

            IEnumerable<Product> products = await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId);

            var productsList = products.ToList();
            productsList.Count.Should().Be(1);

            Product tvFromList = productsList.First();
            tvFromList.Should().BeEquivalentTo(product, DefaultProductEquivalencyOptions);

            tvFromList.ApplySaleDiscount(0.10);
            await _productsRepository.UpdateAsync(tvFromList);

            Product discountedTv = await _productsRepository.GetAsync(product.Id, product.CategoryId);
            discountedTv.Price.Should().Be(450);

            Product? notFoundProduct = await _productsRepository.TryGetAsync(Guid.NewGuid().ToString());
            notFoundProduct.Should().BeNull();

            await _productsRepository.DeleteAsync(discountedTv);

            products = await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId);
            products.Count().Should().Be(0);
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    [Fact(Skip = "In discussing this with Bill, we've decided that this might not be reliable enough to justify having it be a release gate.")]
    public async Task Upsert_WithEtag_WorksCorrectly()
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);

            StockInformation stockInformation = new(5, DateTime.UtcNow);

            Product product = new(
                "Samsung TV",
                TechnologyCategoryId,
                500,
                stockInformation);

            product = await _productsRepository.CreateAsync(product);

            var etagAfterCreation = (await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId)).Single().Etag;

            product.ApplySaleDiscount(0.10);

            Product productToUpsert = new(product.Name, product.CategoryId, product.Price,
                new StockInformation(product.Stock.Count, product.Stock.LastReplenishedUtc,
                    product.Stock.DueReplenishmentUtc))
            {
                Id = product.Id
            };

            await _productsRepository.UpdateAsync(productToUpsert);

            Product updatedProduct =
                (await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId)).Single();

            updatedProduct.Price.Should().Be(product.Price);
            updatedProduct.Etag.Should().NotBe(etagAfterCreation);
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }
}