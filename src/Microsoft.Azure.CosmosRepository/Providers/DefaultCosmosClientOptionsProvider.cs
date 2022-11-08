// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Internals;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc />
class DefaultCosmosClientOptionsProvider : ICosmosClientOptionsProvider
{
    readonly Lazy<CosmosClientOptions> _lazyClientOptions;

    /// <inheritdoc />
    public CosmosClientOptions ClientOptions => _lazyClientOptions.Value;

    /// <summary>
    /// Default <see cref="ICosmosClientOptionsProvider"/> implementation.
    /// </summary>
    /// <param name="serviceProvider">Service provider implementation.</param>
    /// <param name="configuration">Service configuration implementation.</param>
    public DefaultCosmosClientOptionsProvider(
        IServiceProvider serviceProvider,
        IConfiguration configuration) =>
        _lazyClientOptions = new Lazy<CosmosClientOptions>(
            () => CreateCosmosClientOptions(serviceProvider, configuration));

    CosmosClientOptions CreateCosmosClientOptions(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        RepositoryOptions ro = new();
        configuration.GetSection(nameof(RepositoryOptions))
            .Bind(ro);

        CosmosClientOptions cosmosClientOptions = new()
        {
            SerializerOptions = new()
            {
                IgnoreNullValues = ro.SerializationOptions?.IgnoreNullValues ?? false,
                Indented = ro.SerializationOptions?.Indented ?? false,
                PropertyNamingPolicy = ro.SerializationOptions?.PropertyNamingPolicy
                    ?? CosmosPropertyNamingPolicy.CamelCase
            },
            HttpClientFactory = () => ClientFactory(serviceProvider),
            AllowBulkExecution = ro.AllowBulkExecution
        };

        CosmosClientOptionsManipulator manipulator =
            serviceProvider.GetRequiredService<CosmosClientOptionsManipulator>();

        // Allow consumers to control the CosmosClientOptions.
        manipulator.Configure(cosmosClientOptions);

        // TODO: consider throwing an exception if the HttpClientFactory is overridden.

        return cosmosClientOptions;
    }

    static HttpClient ClientFactory(IServiceProvider serviceProvider)
    {
        HttpClient client =
            serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

        string? version =
            Assembly.GetExecutingAssembly()
                    ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;

        client.DefaultRequestHeaders
              .UserAgent
              .ParseAdd($"ievangelist-cosmos-repository-sdk/{version ?? "0.0"}");

        return client;
    }
}