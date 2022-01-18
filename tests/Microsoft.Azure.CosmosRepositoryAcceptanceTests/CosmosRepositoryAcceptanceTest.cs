// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions.Equivalency;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
public abstract class CosmosRepositoryAcceptanceTest
{
    protected const string ProductsInfoContainer = "products-info";
    protected const string DefaultPartitionKey = "/partitionKey";
    protected const string TechnologyCategoryId = "Techonology";

    protected readonly ServiceProvider _provider;
    protected readonly IRepository<Product> _productsRepository;
    protected readonly IRepository<Rating> _ratingsRepository;

    protected EquivalencyAssertionOptions<Product> DefaultProductEquivalencyOptions(
        EquivalencyAssertionOptions<Product> options)
    {
        options.Excluding(x => x.Etag);
        options.Excluding(x => x.CreatedTimeUtc);
        options.Excluding(x => x.LastUpdatedTimeRaw);
        options.Excluding(x => x.LastUpdatedTimeUtc);

        return options;
    }

    protected CosmosRepositoryAcceptanceTest(Action<RepositoryOptions> builderOptions,
        ITestOutputHelper testOutputHelper)
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

    protected static readonly Action<RepositoryOptions> DefaultTestRepositoryOptions = options =>
    {
        ConfigureDatabaseSettings(options);
        ConfigureProducts(options);
        ConfigureRatings(options);
        ConfigureProducts(options);
    };

    protected static readonly Action<RepositoryOptions> ConfigureDatabaseSettings = options =>
    {
        string os = Environment.OSVersion.Platform.ToString().ToLower();
        options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
        options.ContainerPerItemType = true;
        options.DatabaseId = $"cosmos-repository-acceptance-tests-{os}";

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
}