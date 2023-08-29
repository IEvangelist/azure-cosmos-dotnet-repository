// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.



namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Collection("CosmosTest")]
public abstract class CosmosRepositoryAcceptanceTest
{
    protected const string ProductsInfoContainer = "products-info";
    protected const string DefaultPartitionKey = "/partitionKey";
    protected const string TechnologyCategoryId = "Technology";
    protected const string AcceptanceTestsDatabaseSuffix = "cosmos-repo-acceptance-tests-db";

    protected readonly ServiceProvider _provider;
    protected readonly IRepository<Product> _productsRepository;
    protected readonly IRepository<Rating> _ratingsRepository;
    protected readonly ILogger<CosmosRepositoryAcceptanceTest> _logger;

    protected EquivalencyAssertionOptions<Product> DefaultProductEquivalencyOptions(
        EquivalencyAssertionOptions<Product> options)
    {
        options.Excluding(x => x.Etag);
        options.Excluding(x => x.CreatedTimeUtc);
        options.Excluding(x => x.LastUpdatedTimeRaw);
        options.Excluding(x => x.LastUpdatedTimeUtc);

        return options;
    }

    protected CosmosRepositoryAcceptanceTest(
        ITestOutputHelper testOutputHelper,
        Action<RepositoryOptions>? builderOptions = null)
    {
        ConfigurationBuilder config = new();
        config.AddEnvironmentVariables();

        IConfiguration builtConfig = config.Build();

        ServiceCollection services = new();
        services.AddSingleton(builtConfig);

        services.AddCosmosRepository(builderOptions);

        services.AddCosmosRepositoryItemChangeFeedProcessors(typeof(CosmosRepositoryAcceptanceTest).Assembly);

        services.AddLogging(options =>
        {
            options.ClearProviders();
            options.AddXUnit(testOutputHelper, loggerOptions => loggerOptions.Filter = (s, _) =>
                    s is null || !s.StartsWith("System.Net"));

            options.SetMinimumLevel(LogLevel.Debug);
        });

        _provider = services.BuildServiceProvider();

        _logger = _provider.GetRequiredService<ILogger<CosmosRepositoryAcceptanceTest>>();

        var factory = _provider.GetRequiredService<IRepositoryFactory>();

        _productsRepository = factory.RepositoryOf<Product>();
        _ratingsRepository = factory.RepositoryOf<Rating>();
    }

    internal ICosmosClientProvider GetClient() =>
        _provider.GetRequiredService<ICosmosClientProvider>();

    protected static string GetCosmosConnectionString() =>
        Environment.GetEnvironmentVariable("CosmosConnectionString")!;

    protected static readonly Action<RepositoryOptions> DefaultTestRepositoryOptions = options =>
    {
        options.CosmosConnectionString = GetCosmosConnectionString();
        options.ContainerPerItemType = true;
        options.DatabaseId = BuildDatabaseName("products");

        // ReSharper disable once ConstantConditionalAccessQualifier
        ConfigureProducts?.Invoke(options);
        // ReSharper disable once ConstantConditionalAccessQualifier
        ConfigureRatings?.Invoke(options);
    };

    protected static readonly Action<RepositoryOptions> ConfigureDatabaseSettings = options =>
    {
        options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
        options.ContainerPerItemType = true;
        options.DatabaseId = BuildDatabaseName("products");

        // ReSharper disable once ConstantConditionalAccessQualifier
        ConfigureProducts?.Invoke(options);
    };

    protected static readonly Action<RepositoryOptions> ConfigureProducts = options => options.ContainerBuilder.Configure<Product>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });

    protected static readonly Action<RepositoryOptions> ConfigureRatings = options => options.ContainerBuilder.Configure<Rating>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });

    protected async Task<ContainerProperties?> PruneDatabases(CosmosClient client)
    {
        FeedIterator<DatabaseProperties> containerQueryIterator =
            client.GetDatabaseQueryIterator<DatabaseProperties>("SELECT * FROM c");

        while (containerQueryIterator.HasMoreResults)
        {
            foreach (DatabaseProperties database in await containerQueryIterator.ReadNextAsync())
            {
                if (database.Id.EndsWith(AcceptanceTestsDatabaseSuffix))
                {
                    _logger.LogInformation("Deleting database {DatabaseName}", database.Id);
                    await client.GetDatabase(database.Id).DeleteAsync();
                }
            }
        }

        return null;
    }

    protected static async Task<ContainerProperties?> GetContainerProperties(Database database, string containerName)
    {
        FeedIterator<ContainerProperties>? containerQueryIterator =
            database.GetContainerQueryIterator<ContainerProperties>("SELECT * FROM c");

        while (containerQueryIterator.HasMoreResults)
        {
            foreach (ContainerProperties container in await containerQueryIterator.ReadNextAsync())
            {
                if (container.Id == containerName)
                {
                    return container;
                }
            }
        }

        return null;
    }

    protected static string BuildDatabaseName(string prefix) =>
        $"{prefix}-{AcceptanceTestsDatabaseSuffix}";
}