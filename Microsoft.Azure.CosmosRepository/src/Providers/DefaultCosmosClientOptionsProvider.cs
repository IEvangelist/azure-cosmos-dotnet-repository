// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.Providers
{
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
            RepositoryOptions options = new();

            configuration.GetSection(nameof(RepositoryOptions))
                .Bind(options);

            return new()
            {
                SerializerOptions =
                {
                    IgnoreNullValues = options.SerializationOptions.IgnoreNullValues,
                    Indented = options.SerializationOptions.Indented,
                    PropertyNamingPolicy = options.SerializationOptions.PropertyNamingPolicy
                },
                HttpClientFactory = () => ClientFactory(serviceProvider),
                AllowBulkExecution = options.AllowBulkExecution
            };
        }

        static HttpClient ClientFactory(IServiceProvider serviceProvider)
        {
            HttpClient client =
                serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

            string version =
                Assembly.GetExecutingAssembly()
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        .InformationalVersion;

            client.DefaultRequestHeaders
                  .UserAgent
                  .ParseAdd($"ievangelist-cosmos-repository-sdk/{version}");

            return client;
        }
    }
}