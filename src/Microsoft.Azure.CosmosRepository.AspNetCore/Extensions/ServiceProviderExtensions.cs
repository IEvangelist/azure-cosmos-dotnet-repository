using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceProvider"/> to support Cosmos Repository operations.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Eagerly initializes Cosmos DB database and containers before the application starts handling requests.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="assemblies">The assemblies to scan for <see cref="IItem"/> types. Optional. If not provided and no types are explicitly configured, types are discovered from all loaded assemblies.</param>
    /// <returns>The service provider for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method triggers the creation of the Cosmos DB database and containers that would normally
    /// be created lazily on first access when <see cref="RepositoryOptions.IsAutoResourceCreationIfNotExistsEnabled"/>
    /// is enabled.
    /// </para>
    /// <para>
    /// The method discovers item types in two ways:
    /// </para>
    /// <list type="number">
    ///   <item><description>Types explicitly configured via <c>ContainerBuilder.Configure&lt;TItem&gt;()</c></description></item>
    ///   <item><description>Types discovered by scanning assemblies (when assemblies parameter is provided or no types are explicitly configured)</description></item>
    /// </list>
    /// <para>
    /// <strong>Use this method to:</strong>
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Ensure containers exist before health checks run (prevents health check failures during lazy initialization)</description></item>
    ///   <item><description>Avoid latency on the first request to your application</description></item>
    ///   <item><description>Fail fast during startup if there are Cosmos DB configuration or connectivity issues</description></item>
    /// </list>
    /// <para>
    /// <strong>Important:</strong> If <see cref="RepositoryOptions.IsAutoResourceCreationIfNotExistsEnabled"/>
    /// is <c>false</c>, this method does nothing and returns immediately. The database and containers must
    /// already exist in that case.
    /// </para>
    /// <para>
    /// <strong>Timing:</strong> Call this method after <c>Build()</c> but before <c>Run()</c> in your
    /// application startup code. Containers will still be created on first use when auto-creation is enabled
    /// even if this method is not called - this just moves the timing from first-access to startup.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.Services.AddCosmosRepository(options =>
    /// {
    ///     options.ContainerBuilder.Configure&lt;Product&gt;(c => c.WithContainer("products"));
    /// });
    /// builder.Services.AddHealthChecks().AddCosmosRepository();
    ///
    /// var app = builder.Build();
    ///
    /// // Eagerly initialize containers before starting the application
    /// // Will discover Product from explicit configuration
    /// await app.Services.EagerlyInitializeCosmosContainersAsync();
    ///
    /// app.MapHealthChecks("/health");
    /// app.Run();
    /// </code>
    /// </example>
    public static async Task<IServiceProvider> EagerlyInitializeCosmosContainersAsync(
        this IServiceProvider serviceProvider,
        params Assembly[]? assemblies)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ICosmosContainerService>>();
        var options = serviceProvider.GetRequiredService<IOptions<RepositoryOptions>>();

        if (!options.Value.IsAutoResourceCreationIfNotExistsEnabled)
        {
            logger.LogDebug(
                "Skipping eager Cosmos container initialization because {PropertyName} is false. " +
                "Database and containers must already exist.",
                nameof(options.Value.IsAutoResourceCreationIfNotExistsEnabled));
            return serviceProvider;
        }

        var itemConfigProvider = serviceProvider.GetRequiredService<ICosmosItemConfigurationProvider>();
        var containerService = serviceProvider.GetRequiredService<ICosmosContainerService>();

        // Get explicitly configured types from ContainerBuilder
        var explicitlyConfiguredTypes = options.Value.ContainerOptions
            .Select(co => co.Type)
            .ToList();

        List<ItemConfiguration> itemConfigs;

        if (explicitlyConfiguredTypes.Any())
        {
            // Use explicitly configured types
            logger.LogDebug(
                "Using {Count} explicitly configured item type(s) from ContainerBuilder",
                explicitlyConfiguredTypes.Count);

            itemConfigs = explicitlyConfiguredTypes
                .Select(type => itemConfigProvider.GetItemConfiguration(type))
                .ToList();
        }
        else if (assemblies is {Length: > 0})
        {
            // Scan provided assemblies
            logger.LogDebug(
                "Scanning {Count} provided assemblies for item types",
                assemblies.Length);

            itemConfigs = itemConfigProvider.GetAllItemConfigurations(assemblies).ToList();
        }
        else
        {
            // Scan all assemblies as fallback
            logger.LogDebug(
                "No explicit configuration or assemblies provided. Scanning all loaded assemblies for item types");

            itemConfigs = itemConfigProvider.GetAllItemConfigurations().ToList();
        }

        var distinctContainers = itemConfigs.Select(c => c.ContainerName).Distinct().ToList();

        logger.LogInformation(
            "Eagerly initializing {ContainerCount} Cosmos DB container(s) for {ItemTypeCount} item type(s). " +
            "Without this call, containers would be created lazily on first repository access.",
            distinctContainers.Count,
            itemConfigs.Count);

        foreach (var config in itemConfigs)
        {
            try
            {
                await containerService.GetContainerAsync([config.Type]);
                logger.LogDebug(
                    "Successfully initialized container '{ContainerName}' for item type '{ItemType}'",
                    config.ContainerName,
                    config.Type.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to initialize container '{ContainerName}' for item type '{ItemType}'. " +
                    "Error: {ErrorMessage}",
                    config.ContainerName,
                    config.Type.Name,
                    ex.Message);
                throw;
            }
        }

        logger.LogInformation(
            "Successfully completed eager initialization of {ContainerCount} Cosmos DB container(s)",
            distinctContainers.Count);

        return serviceProvider;
    }
}