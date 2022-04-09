// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class InMemoryRepositoryBasicsTests : CosmosRepositoryInMemoryAcceptanceTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public InMemoryRepositoryBasicsTests(ITestOutputHelper testOutputHelper) :
        base(testOutputHelper, DefaultTestInMemoryRepositoryOptions)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task ProductRepository_BasicCRUDOperations_WorksCorrectly()
    {
        StockInformation stockInformation = new(5, DateTime.UtcNow);

        Product product = new(
            Guid.NewGuid().ToString(),
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

        Product? notFoundProduct = await _productsRepository.TryGetAsync(Guid.NewGuid().ToString());
        notFoundProduct.Should().BeNull();

        await _productsRepository.DeleteAsync(discountedTv);

        products = await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId);
        products.Count().Should().Be(0);
    }

    [Fact]
    public async Task Upsert_WithEtag_WorksCorrectlyWhenOptimizeBandwidthIsFalse()
    {
        void SetEtag(Product product, string? etag)
        {
            Type t = product.GetType();
            t.BaseType!.InvokeMember("Etag", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, product, new object[] { etag });
        }

        StockInformation stockInformation = new(5, DateTime.UtcNow);

        Product product = new(
            "Samsung TV",
            TechnologyCategoryId,
            500,
            stockInformation);

        product = await _productsRepository.CreateAsync(product);

        string? etagAfterCreation = (await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId)).Single().Etag;
        etagAfterCreation.Should().Be(product.Etag);


        // First Update - Etag
        product.ApplySaleDiscount(0.10);

        product = await _productsRepository.UpdateAsync(product);

        string? etagAfterUpdate = (await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId)).Single().Etag;
        etagAfterUpdate.Should().Be(product.Etag);

        // Second Update - Null Etag
        SetEtag(product, null);
        product.ApplySaleDiscount(0.10);

        product = await _productsRepository.UpdateAsync(product);

        etagAfterUpdate = (await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId)).Single().Etag;
        etagAfterUpdate.Should().Be(product.Etag);

        // Third Update - Invalid Etag
        SetEtag(product, Guid.NewGuid().ToString());
        product.ApplySaleDiscount(0.10);

        try
        {
            product = await _productsRepository.UpdateAsync(product);
            true.Should().BeFalse();
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.PreconditionFailed)
        {

        }

        // Third Update - Second Attempt - Invalid Etag - Ignore Etag
        product = await _productsRepository.UpdateAsync(product, ignoreEtag: true);

        etagAfterUpdate = (await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId)).Single().Etag;
        etagAfterUpdate.Should().Be(product.Etag);


        Product finalProduct =
            (await _productsRepository.GetAsync(x => x.PartitionKey == TechnologyCategoryId)).Single();

        finalProduct.Price.Should().Be(product.Price);
        finalProduct.Etag.Should().Be(product.Etag);
        finalProduct.Etag.Should().NotBe(etagAfterCreation);
    }
}