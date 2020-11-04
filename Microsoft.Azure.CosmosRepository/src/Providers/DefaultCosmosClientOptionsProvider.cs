using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultCosmosClientOptionsProvider : ICosmosClientOptionsProvider
    {
        private readonly Lazy<CosmosClientOptions> _lazyClientOptions;

        /// <inheritdoc />
        public CosmosClientOptions GetClientOptions => _lazyClientOptions.Value;

        /// <summary>
        /// Default <see cref="ICosmosClientOptionsProvider"/> implementation.
        /// </summary>
        /// <param name="serviceProvider">Service provider implementation.</param>
        /// <param name="configuration">Service configuration implementation.</param>
        public DefaultCosmosClientOptionsProvider(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _lazyClientOptions = new Lazy<CosmosClientOptions>(() => CreateCosmosClientOptions(serviceProvider, configuration));
        }

        private CosmosClientOptions CreateCosmosClientOptions(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            IHttpClientFactory factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            return new CosmosClientOptions
                   {
                       HttpClientFactory = () =>
                                           {
                                               HttpClient client = factory.CreateClient();

                                               string version =
                                                   Assembly.GetExecutingAssembly()
                                                           .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                           .InformationalVersion;

                                               client.DefaultRequestHeaders
                                                     .UserAgent
                                                     .ParseAdd($"ievangelist-cosmos-repository-sdk/{version}");

                                               return client;
                                           },
                       AllowBulkExecution = configuration.GetSection(nameof(RepositoryOptions))
                                                         .GetValue<bool>(nameof(RepositoryOptions.AllowBulkExecution))
                   };
        }
    }
}