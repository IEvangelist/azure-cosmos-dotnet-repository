// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosRepository.Exceptions;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class TransactionalBatchTests : CosmosRepositoryAcceptanceTest
{
    public TransactionalBatchTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper, DefaultTestRepositoryOptions)
    {
    }

    [Fact]
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