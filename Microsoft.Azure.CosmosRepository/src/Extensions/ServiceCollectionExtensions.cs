// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the services required to consume any number of <see cref="IRepository{TDocument}"/> 
        /// instances to interact with Cosmos DB.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The configuration representing the applications settings.</param>
        /// <returns>The same service collection that was provided, with the required cosmos services.</returns>
        public static IServiceCollection AddCosmosRepository(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(
                    nameof(services), "A service collection is required.");
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(
                    nameof(configuration), "A configuration is required.");
            }

            if (configuration[nameof(RepositoryOptions)] is null)
            {
                throw new ArgumentNullException(
                    nameof(configuration), $"The {nameof(RepositoryOptions)} configuration section is required.");
            }

            return services.AddLogging()
                .AddSingleton<ICosmosContainerProvider, DefaultCosmosContainerProvider>()
                .AddSingleton(typeof(IRepository<>), typeof(DefaultRepository<>))
                .Configure<RepositoryOptions>(
                    configuration.GetSection(nameof(RepositoryOptions)));
        }
    }
}