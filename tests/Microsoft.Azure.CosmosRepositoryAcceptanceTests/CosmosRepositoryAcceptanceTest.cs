// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using FluentAssertions.Equivalency;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Collection("CosmosTest")]
public abstract class CosmosRepositoryAcceptanceTest
{
    protected const string ProductsInfoContainer = "products-info";
    protected const string ProductsDatabase = "inventory";
    protected const string DefaultPartitionKey = "/partitionKey";
    protected const string TechnologyCategoryId = "Techonology";

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
            options.AddXUnit(testOutputHelper, loggerOptions =>
            {
                loggerOptions.Filter = (s, _) =>
                    s is null || !s.StartsWith("System.Net");
            });

            options.SetMinimumLevel(LogLevel.Debug);
        });

        _provider = services.BuildServiceProvider();

        _productsRepository = _provider.GetRequiredService<IRepository<Product>>();
        _ratingsRepository = _provider.GetRequiredService<IRepository<Rating>>();
        _logger = _provider.GetRequiredService<ILogger<CosmosRepositoryAcceptanceTest>>();
    }

    protected static async Task WaitFor(int seconds)
    {
        int counter = 0;

        while (counter < seconds)
        {
            counter++;
            await Task.Delay(3000);
        }
    }

    internal ICosmosClientProvider GetClient() =>
        _provider.GetRequiredService<ICosmosClientProvider>();

    protected static string GetCosmosConnectionString() =>
        Environment.GetEnvironmentVariable("CosmosConnectionString")!;

    protected static readonly Action<RepositoryOptions> DefaultTestRepositoryOptions = options =>
    {
        options.CosmosConnectionString = GetCosmosConnectionString();
        options.ContainerPerItemType = true;
        options.DatabaseId = ProductsDatabase;

        ConfigureProducts(options);
        ConfigureRatings(options);

        ConfigureProducts(options);
    };

    protected static readonly Action<RepositoryOptions> ConfigureDatabaseSettings = options =>
    {
        options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
        options.ContainerPerItemType = true;
        options.DatabaseId = "cosmos-repository-acceptance-tests";

        ConfigureProducts(options);
    };

    protected static readonly Action<RepositoryOptions> ConfigureProducts = options =>
    {
        options.ContainerBuilder.Configure<Product>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });
    };

    protected static readonly Action<RepositoryOptions> ConfigureRatings = options =>
    {
        options.ContainerBuilder.Configure<Rating>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });
    };


    protected async Task<ContainerProperties?> DeleteDatabaseIfExists(string dbName, CosmosClient client)
    {
        FeedIterator<DatabaseProperties> containerQueryIterator =
            client.GetDatabaseQueryIterator<DatabaseProperties>("SELECT * FROM c");

        while (containerQueryIterator.HasMoreResults)
        {
            foreach (DatabaseProperties database in await containerQueryIterator.ReadNextAsync())
            {
                if (database.Id == dbName)
                {
                    _logger.LogInformation("Deleting database {DatabaseName}", dbName);
                    await client.GetDatabase(dbName).DeleteAsync();
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
}