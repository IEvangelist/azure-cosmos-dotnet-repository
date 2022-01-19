// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
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

[Trait("Type", "Functional")]
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
            builder.WithChangeFeedMonitoring(feedOptions => feedOptions.PollInterval = TimeSpan.FromSeconds(1));
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

    public ChangeFeedBasicTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, ChangeFeedTestOptions)
    {
        _ratingsByCategoryRepository = _provider.GetRequiredService<IRepository<RatingByCategory>>();
        _changeFeedService = _provider.GetRequiredService<IChangeFeedService>();
    }

    [Fact]
    public async Task Create_Rating_For_Product_Is_Replicated_To_Be_Partitioned_By_Category()
    {
        try
        {
            await _changeFeedService.StartAsync(default);

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

            await WaitFor(1);

            IEnumerable<RatingByCategory> results = await _ratingsByCategoryRepository
                .GetAsync(x => x.PartitionKey == TechnologyCategoryId &&
                               x.ProductId == product.Id);

            results.Count().Should().Be(1);
        }
        finally
        {
            await _changeFeedService.StopAsync();
        }
    }
}