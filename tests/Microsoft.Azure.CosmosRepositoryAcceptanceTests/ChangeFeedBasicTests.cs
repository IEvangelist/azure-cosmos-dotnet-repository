// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

public class ChangeFeedBasicTests : CosmosRepositoryAcceptanceTest
{
    private static readonly Action<RepositoryOptions> ChangeFeedTestOptions = options =>
    {
        ConfigureDatabaseSettings(options);
        ConfigureProducts(options);

        options.ContainerBuilder.Configure<Rating>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
            builder.WithChangeFeedMonitoring();
        });

        options.ContainerBuilder.Configure<RatingByCategory>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });
    };

    private readonly IRepository<RatingByCategory> _ratingsByCategoryRepository;
    private readonly IChangeFeedService _changeFeedService;

    public ChangeFeedBasicTests(ITestOutputHelper testOutputHelper) : base(ChangeFeedTestOptions, testOutputHelper)
    {
        _ratingsByCategoryRepository = _provider.GetRequiredService<IRepository<RatingByCategory>>();
        _changeFeedService = _provider.GetRequiredService<IChangeFeedService>();
    }

    [Fact]
    public async Task Create_Rating_For_Product_Is_Replicated_To_Be_Partitioned_By_Category()
    {
        StockInformation stockInformation = new(5, DateTime.UtcNow);

        Product product = new(
            "Samsung TV",
            TechnologyCategoryId,
            500,
            stockInformation);

        await _productsRepository.CreateAsync(product);

        Rating tvRating = new(
            product.Id,
            3,
            "Very good product",
            product.CategoryId);

        await _ratingsRepository.CreateAsync(tvRating);

        await _changeFeedService.StartAsync(default);

        await Task.Delay(10000);

        try
        {
            bool result = await _ratingsByCategoryRepository.ExistsAsync(tvRating.Id, TechnologyCategoryId);
            result.Should().BeTrue();
        }
        finally
        {
            await _changeFeedService.StopAsync();
        }
    }
}