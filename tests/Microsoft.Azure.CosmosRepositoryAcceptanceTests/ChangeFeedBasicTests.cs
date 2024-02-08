// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.



namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
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
            builder.WithChangeFeedMonitoring(feedOptions => feedOptions.PollInterval = TimeSpan.FromMilliseconds(500));
        });

        options.ContainerBuilder.Configure<RatingByCategory>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });
    };

    private readonly AsyncPolicy _readCountPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(5, i => TimeSpan.FromSeconds(i * 2));

    private readonly IRepository<RatingByCategory> _ratingsByCategoryRepository;
    private readonly IChangeFeedService _changeFeedService;

    public ChangeFeedBasicTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, ChangeFeedTestOptions)
    {
        _ratingsByCategoryRepository = _provider.GetRequiredService<IRepository<RatingByCategory>>();
        _changeFeedService = _provider.GetRequiredService<IChangeFeedService>();
    }

    [Fact(Skip = "In discussing this with Bill, we've decided that this might not be reliable enough to justify having it be a release gate.")]
    public async Task Create_Rating_For_Product_Is_Replicated_To_Be_Partitioned_By_Category()
    {
        try
        {
            await GetClient().UseClientAsync(PruneDatabases);
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

            await _readCountPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Checking projections");
                IEnumerable<RatingByCategory> results = await _ratingsByCategoryRepository
                    .GetAsync(x => x.PartitionKey == TechnologyCategoryId &&
                                   x.ProductId == product.Id);

                results.Count().Should().Be(1);
            });
        }
        finally
        {
            await _changeFeedService.StopAsync();
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }
}