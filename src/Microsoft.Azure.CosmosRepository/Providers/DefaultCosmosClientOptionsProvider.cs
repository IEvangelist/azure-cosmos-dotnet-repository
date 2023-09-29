// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

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
            SerializerOptions = ro.SerializationOptions,
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

        var version =
            Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion
                    ?? "0.0";

        client.DefaultRequestHeaders
              .UserAgent
              .ParseAdd($"ievangelist-cosmos-repository-sdk/{version}");

        return client;
    }
}