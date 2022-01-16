// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
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

    protected CosmosRepositoryAcceptanceTest(Action<RepositoryOptions> builderOptions, ITestOutputHelper testOutputHelper)
    {
        ConfigurationBuilder config = new();
        config.AddEnvironmentVariables();

        ServiceCollection services = new();
        services.AddSingleton<IConfiguration>(config.Build());
        services.AddCosmosRepository(builderOptions);
        services.AddCosmosRepositoryItemChangeFeedProcessors();

        services.AddLogging(options =>
        {
            options.AddXUnit(testOutputHelper);
            options.SetMinimumLevel(LogLevel.Debug);
        });

        _provider = services.BuildServiceProvider();

        _productsRepository = _provider.GetRequiredService<IRepository<Product>>();
        _ratingsRepository = _provider.GetRequiredService<IRepository<Rating>>();
    }

    protected static readonly Action<RepositoryOptions> DefaultTestRepositoryOptions = options =>
    {
        options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
        options.ContainerPerItemType = true;
        options.DatabaseId = "cosmos-repository-acceptance-tests";

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
}