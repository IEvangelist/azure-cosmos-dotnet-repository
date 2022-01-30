// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;
using Microsoft.Azure.CosmosRepository.Specification.Builder;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Functional")]
public class SpecificationQueryTests : CosmosRepositoryAcceptanceTest
{
    private readonly List<Product> _products = new()
    {
        new Product("iPad", TechnologyCategoryId, 450, Stock(5)),
        new Product("iPhone", TechnologyCategoryId, 650, Stock(5)),
        new Product("Scarf", "Clothing", 10, Stock(5)),
        new Product("Jumper", "Clothing", 25, Stock(5)),
        new Product("Socks", "Clothing", 5, Stock(5)),
    };

    private static StockInformation Stock(int count = 2) => new(count, DateTime.UtcNow);

    public SpecificationQueryTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper, DefaultTestRepositoryOptions)
    {
    }

    [Fact]
    public async Task Query_SpecificationWithOrderByDescending_OrdersItemsCorrectly()
    {
        try
        {
            //Arrange
            await _productsRepository.CreateAsync(_products);

            //Act
            IQueryResult<Product> orderedProducts =
                await _productsRepository.QueryAsync(new ProductsPriceHighestToLowest());

            //Assert
            orderedProducts.Items.Count.Should().Be(_products.Count);
            orderedProducts.Items[0].Name.Should().Be("iPhone");
            orderedProducts.Items.Last().Name.Should().Be("Socks");
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    [Fact]
    public async Task Query_SpecificationWithOrderBy_OrdersItemsCorrectly()
    {
        try
        {
            //Arrange
            await _productsRepository.CreateAsync(_products);

            //Act
            IQueryResult<Product> orderedProducts =
                await _productsRepository.QueryAsync(new ProductsPriceLowestToHighest());

            //Assert
            orderedProducts.Items.Count.Should().Be(_products.Count);
            orderedProducts.Items[0].Name.Should().Be("Socks");
            orderedProducts.Items.Last().Name.Should().Be("iPhone");
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    [Fact]
    public async Task Query_SpecificationWithOrderByAndFilter_OrdersItemsCorrectly()
    {
        try
        {
            //Arrange
            await _productsRepository.CreateAsync(_products);

            //Act
            IQueryResult<Product> orderedProducts =
                await _productsRepository.QueryAsync(new ProductsPriceLowestToHighestInCategory("Clothing"));

            //Assert
            orderedProducts.Items.Count.Should().Be(_products.Count(x => x.CategoryId == "Clothing"));
            orderedProducts.Items[0].Name.Should().Be("Socks");
            orderedProducts.Items.Last().Name.Should().Be("Jumper");
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    [Fact]
    public async Task Query_SpecificationWithOrderByAndFilterAndPagingUsingContinuationTokens_BehavesCorrectly()
    {
        try
        {
            //Arrange
            await _productsRepository.CreateAsync(_products);

            ProductsInCategoryLowestToHighestPagedUsingTokens specification = new("Clothing");

            //Act
            IPage<Product> orderedProductsPage1 =
                await _productsRepository.QueryAsync<IPage<Product>>(specification);

            specification.UpdateContinuationToken(orderedProductsPage1.Continuation);

            IPage<Product> orderedProductsPage2 =
                await _productsRepository.QueryAsync<IPage<Product>>(specification);

            //Assert
            orderedProductsPage1.Items.Count.Should().Be(2);
            orderedProductsPage1.Continuation.Should().NotBeNull();
            orderedProductsPage1.Items[0].Name.Should().Be("Socks");
            orderedProductsPage1.Items[1].Name.Should().Be("Scarf");
            orderedProductsPage1.Total.Should().Be(3);

            orderedProductsPage2.Items.Count.Should().Be(1);
            orderedProductsPage2.Continuation.Should().BeNull();
            orderedProductsPage2.Items[0].Name.Should().Be("Jumper");
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    private class ProductsPriceHighestToLowest : ListSpecification<Product>
    {
        public ProductsPriceHighestToLowest() =>
            Query.OrderByDescending(x => x.Price);
    }

    private class ProductsPriceLowestToHighest : ListSpecification<Product>
    {
        public ProductsPriceLowestToHighest() =>
            Query.OrderBy(x => x.Price);
    }

    private class ProductsPriceLowestToHighestInCategory : ListSpecification<Product>
    {
        public ProductsPriceLowestToHighestInCategory(string categoryId) =>
            Query.Where(x => x.PartitionKey == categoryId)
                .OrderBy(x => x.Price);
    }

    private class ProductsInCategoryLowestToHighestPagedUsingTokens : ContinuationTokenSpecification<Product>
    {
        public ProductsInCategoryLowestToHighestPagedUsingTokens(string category, int pageSize = 2) =>
            Query.Where(x => x.PartitionKey == category)
                .OrderBy(x => x.Price)
                .PageSize(pageSize);

    }
}