// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;

/// <summary>
/// Extension methods for adding and configuring the Azure Cosmos DB services when running on ASP.NET CORE.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the hosted service to process changes from any number of cosmos containers.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same service collection that was provided, with the <see cref="CosmosRepositoryChangeFeedHostedService"/></returns>
    /// <remarks>Please ensure this is called in conjunction with services.AddCosmosRepository(...)</remarks>
    public static IServiceCollection AddCosmosRepositoryChangeFeedHostedService(this IServiceCollection services)
    {
        services.AddHostedService<CosmosRepositoryChangeFeedHostedService>();
        return services;
    }

    /// <summary>
    /// Adds the given <see cref="IItemChangeFeedProcessor{TItem}"/> defined in the given assemblies.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="assemblies">The assemblies to scan for <see cref="IItemChangeFeedProcessor{TItem}"/>'s.</param>
    /// <returns>The same service collection that was provided, with the found <see cref="IItemChangeFeedProcessor{TItem}"/>'s registered as singletons.</returns>
    public static IServiceCollection AddCosmosRepositoryItemChangeFeedProcessors(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IItemChangeFeedProcessor<>)))
            .AsImplementedInterfaces().WithSingletonLifetime());

        return services;
    }
}