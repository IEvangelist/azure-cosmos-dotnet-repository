// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using FluentAssertions.Equivalency;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

public abstract class CosmosRepositoryAcceptanceTest
{
    private const string ProductsInfoContainer = "products-info";
    private const string DefaultPartitionKey = "/partitionKey";
    protected const string TechnologyCategoryId = "Techonology";

    protected readonly IRepository<Product> _productsRepository;
    protected readonly IRepository<Rating> _ratingsRepository;

    protected EquivalencyAssertionOptions<Product> DefaultProductEquivalencyOptions(EquivalencyAssertionOptions<Product> options)
    {
        options.Excluding(x => x.Etag);
        options.Excluding(x => x.CreatedTimeUtc);
        options.Excluding(x => x.LastUpdatedTimeRaw);
        options.Excluding(x => x.LastUpdatedTimeUtc);

        return options;
    }

    protected CosmosRepositoryAcceptanceTest(Action<RepositoryOptions> builderOptions)
    {
        ConfigurationBuilder config = new();
        config.AddUserSecrets<CosmosRepositoryAcceptanceTest>();

        ServiceCollection services = new();
        services.AddSingleton<IConfiguration>(config.Build());
        services.AddCosmosRepository(builderOptions);

        ServiceProvider provider = services.BuildServiceProvider();

        _productsRepository = provider.GetRequiredService<IRepository<Product>>();
        _ratingsRepository = provider.GetRequiredService<IRepository<Rating>>();
    }

    protected static readonly Action<RepositoryOptions> DefaultTestRepositoryOptions = options =>
    {
        options.ContainerPerItemType = true;
        options.DatabaseId = "cosmos-repository-acceptance-tests";

        options.ContainerBuilder.Configure<Product>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });

        options.ContainerBuilder.Configure<Rating>(builder =>
        {
            builder.WithContainer(ProductsInfoContainer);
            builder.WithPartitionKey(DefaultPartitionKey);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });
    };
}