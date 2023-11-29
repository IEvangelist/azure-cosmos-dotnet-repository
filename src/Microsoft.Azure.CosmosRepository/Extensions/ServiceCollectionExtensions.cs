// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding and configuring the Azure Cosmos DB services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required to consume any number of <see cref="IRepository{TItem}"/>
    /// instances to interact with Cosmos DB.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="setupAction">An action to configure the repository options</param>
    /// <param name="additionalSetupAction">An action to configure the <see cref="CosmosClientOptions"/></param>
    /// <returns>The same service collection that was provided, with the required cosmos services.</returns>
    public static IServiceCollection AddCosmosRepository(
        this IServiceCollection services,
        Action<RepositoryOptions>? setupAction = default,
        Action<CosmosClientOptions>? additionalSetupAction = default)
    {
        if (services is null)
        {
            throw new ArgumentNullException(
                nameof(services), "A service collection is required.");
        }

        services.AddOptions<RepositoryOptions>()
            .Configure<IConfiguration>(
                (settings, configuration) =>
                    configuration.GetSection(nameof(RepositoryOptions)).Bind(settings));

        services.AddLogging()
            .AddHttpClient()
            .AddSingleton(new CosmosClientOptionsManipulator(additionalSetupAction))
            .AddSingleton<ICosmosClientOptionsProvider, DefaultCosmosClientOptionsProvider>()
            .AddSingleton<ICosmosClientProvider, DefaultCosmosClientProvider>()
            .AddSingleton(typeof(ICosmosContainerProvider<>), typeof(DefaultCosmosContainerProvider<>))
            .AddSingleton<ICosmosPartitionKeyPathProvider, DefaultCosmosPartitionKeyPathProvider>()
            .AddSingleton<ICosmosContainerNameProvider, DefaultCosmosContainerNameProvider>()
            .AddSingleton<ICosmosUniqueKeyPolicyProvider, DefaultCosmosUniqueKeyPolicyProvider>()
            .AddSingleton(typeof(IReadOnlyRepository<>), typeof(DefaultRepository<>))
            .AddSingleton(typeof(IWriteOnlyRepository<>), typeof(DefaultRepository<>))
            .AddSingleton(typeof(IBatchRepository<>), typeof(DefaultRepository<>))
            .AddSingleton(typeof(IRepository<>), typeof(DefaultRepository<>))
            .AddSingleton<IRepositoryFactory, DefaultRepositoryFactory>()
            .AddSingleton<ICosmosItemConfigurationProvider, DefaultCosmosItemConfigurationProvider>()
            .AddSingleton<ICosmosQueryableProcessor, DefaultCosmosQueryableProcessor>()
            .AddSingleton<IRepositoryExpressionProvider, DefaultRepositoryExpressionProvider>()
            .AddSingleton<IRepositoryOptionsValidator, DefaultRepositoryOptionsValidator>()
            .AddSingleton<ISpecificationEvaluator, SpecificationEvaluator>()
            .AddSingleton<ICosmosContainerDefaultTimeToLiveProvider,
                DefaultCosmosContainerDefaultTimeToLiveProvider>()
            .AddSingleton<ICosmosContainerSyncContainerPropertiesProvider,
                DefaultContainerSyncContainerPropertiesProvider>()
            .AddSingleton<ICosmosContainerService, DefaultCosmosContainerService>()
            .AddSingleton<ICosmosContainerSyncService, DefaultCosmosContainerSyncService>()
            .AddSingleton<ICosmosThroughputProvider, DefaultCosmosThroughputProvider>()
            .AddSingleton<IChangeFeedContainerProcessorProvider, DefaultChangeFeedContainerProcessorProvider>()
            .AddSingleton<IChangeFeedService, DefaultChangeFeedService>()
            .AddSingleton<ILeaseContainerProvider, DefaultLeaseContainerProvider>()
            .AddSingleton<IChangeFeedOptionsProvider, DefaultChangeFeedOptionsProvider>()
            .AddSingleton<ICosmosStrictTypeCheckingProvider, DefaultCosmosStrictTypeCheckingProvider>();

        if (setupAction != default)
        {
            services.PostConfigure(setupAction);
        }

        return services;
    }

    /// <summary>
    /// Adds the services required to run the in memory implementation of the cosmos repository.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns></returns>
    public static IServiceCollection AddInMemoryCosmosRepository(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(
                nameof(services), "A service collection is required.");
        }

        services
            .AddSingleton(typeof(IReadOnlyRepository<>), typeof(InMemoryRepository<>))
            .AddSingleton(typeof(IWriteOnlyRepository<>), typeof(InMemoryRepository<>))
            .AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>))
            .AddSingleton<IRepositoryFactory, DefaultRepositoryFactory>()
            .AddSingleton(typeof(InMemoryChangeFeed<>));

        return services;
    }

    /// <summary>
    /// Remove all of <see cref="IRepository{TItem}"/> from the container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns></returns>
    public static IServiceCollection RemoveCosmosRepositories(this IServiceCollection services)
    {
        var repositories =
            services.Where(i => i.ServiceType == typeof(IRepository<>))
            .ToList();

        repositories.ForEach(r => services.Remove(r));
        return services;
    }
}