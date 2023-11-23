// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using HealthChecks.CosmosDb;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;

public static class HealthChecksBuilderExtensions
{
    //todo: add overloads to pass health check options (tags etc)
    public static IHealthChecksBuilder AddAzureCosmosDB(this IHealthChecksBuilder builder)
    {
        builder.AddAzureCosmosDB(
            clientFactory: provider => provider.GetRequiredService<ICosmosClientProvider>().CosmosClient,
            optionsFactory: sp =>
            {
                IOptions<RepositoryOptions> options = sp.GetRequiredService<IOptions<RepositoryOptions>>();
                ICosmosItemConfigurationProvider itemConfigProvider = sp.GetRequiredService<ICosmosItemConfigurationProvider>();
                IEnumerable<string> containers = itemConfigProvider.GetAllItemConfigurations().Select(i => i.ContainerName).Distinct();

                return new AzureCosmosDbHealthCheckOptions
                {
                    DatabaseId = options.Value.DatabaseId,
                    ContainerIds = containers
                };
            });

        return builder;
    }
}