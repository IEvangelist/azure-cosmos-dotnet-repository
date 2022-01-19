// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

public class RepositoryBasicsTests : CosmosRepositoryAcceptanceTest
{
    public RepositoryBasicsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, DefaultTestRepositoryOptions)
    {
    }

    [Fact]
    public async Task ProductRepository_BasicCRUDOperations_WorksCorrectly()
    {
        StockInformation stockInformation = new(5, DateTime.UtcNow);

        Product product = new(
            "Samsung TV",
            TechnologyCategoryId,
            500,
            stockInformation);

        await _productsRepository.CreateAsync(product);

        IEnumerable<Product> products = await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId);

        List<Product> productsList = products.ToList();
        productsList.Count.Should().Be(1);

        Product tvFromList = productsList.First();
        tvFromList.Should().BeEquivalentTo(product, DefaultProductEquivalencyOptions);

        tvFromList.ApplySaleDiscount(0.10);
        await _productsRepository.UpdateAsync(tvFromList);

        Product discountedTv = await _productsRepository.GetAsync(product.Id, product.CategoryId);
        discountedTv.Price.Should().Be(450);

        await _productsRepository.DeleteAsync(discountedTv);

        products = await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId);
        products.Count().Should().Be(0);
    }
}