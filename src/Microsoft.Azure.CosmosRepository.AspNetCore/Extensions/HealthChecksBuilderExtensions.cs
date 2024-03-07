// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HealthChecks.CosmosDb;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;

public static class HealthChecksBuilderExtensions
{
    /// <summary>
    /// Add a health check for Azure Cosmos DB by registering <see cref="AzureCosmosDbHealthCheck"/> for given <paramref name="builder"/> Use this overload to automatically scan assemblies for configured containerIds.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/> to add <see cref="HealthCheckRegistration"/> to.</param>
    /// <param name="healthCheckName">The health check name. Optional. If <c>null</c> the name 'azure_cosmosdb' will be used.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
    /// <param name="assemblies">The assemblies to scan for <see cref="IItem"/> types. Optional. If <c>null</c> types are discovered autimatically. Providing a assemblies to scan may reduce start up time.</param>
    /// <returns>The specified <paramref name="builder"/>.</returns>
    public static IHealthChecksBuilder AddCosmosRepository(this IHealthChecksBuilder builder,
        string? healthCheckName = "azure_cosmosdb",
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default,
        params Assembly[]? assemblies)
    {
        builder.AddAzureCosmosDB(
            clientFactory: provider => provider.GetRequiredService<ICosmosClientProvider>().CosmosClient,
            optionsFactory: sp =>
            {
                IOptions<RepositoryOptions> options = sp.GetRequiredService<IOptions<RepositoryOptions>>();
                ICosmosItemConfigurationProvider itemConfigProvider =
                    sp.GetRequiredService<ICosmosItemConfigurationProvider>();
                IEnumerable<string> containers = itemConfigProvider.GetAllItemConfigurations(assemblies)
                    .Select(i => i.ContainerName).Distinct();

                return new AzureCosmosDbHealthCheckOptions
                {
                    DatabaseId = options.Value.DatabaseId,
                    ContainerIds = containers
                };
            }, healthCheckName, failureStatus, tags, timeout);

        return builder;
    }

    /// <summary>
    /// Add a health check for Azure Cosmos DB by registering <see cref="AzureCosmosDbHealthCheck"/> for given <paramref name="builder"/>. Use this overload to configure <see cref="AzureCosmosDbHealthCheckOptions"/> to customise which containers to check.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/> to add <see cref="HealthCheckRegistration"/> to.</param>
    /// <param name="healthCheckName">The health check name. Optional. If <c>null</c> the name 'azure_cosmosdb' will be used.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
    /// <param name="optionsFactory">The `AzureCosmosDbHealthCheckOptions` to use. Optional. If <c>null</c>, the health check only calls CosmosClient.ReadAccountAsync.</param>
    /// <returns>The specified <paramref name="builder"/>.</returns>
    public static IHealthChecksBuilder AddCosmosRepository(this IHealthChecksBuilder builder,
        string? healthCheckName = "azure_cosmosdb",
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default,
        Func<IServiceProvider, AzureCosmosDbHealthCheckOptions>? optionsFactory = default
    )
    {
        builder.AddAzureCosmosDB(
            clientFactory: provider => provider.GetRequiredService<ICosmosClientProvider>().CosmosClient,
            optionsFactory: optionsFactory, healthCheckName, failureStatus, tags, timeout);

        return builder;
    }
}