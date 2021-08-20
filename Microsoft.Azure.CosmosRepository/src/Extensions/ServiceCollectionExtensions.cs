// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Internals;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository.Validators;

namespace Microsoft.Extensions.DependencyInjection
{
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
        /// <param name="additionSetupAction">An action to configure the <see cref="CosmosClientOptions"/></param>
        /// <returns>The same service collection that was provided, with the required cosmos services.</returns>
        public static IServiceCollection AddCosmosRepository(
            this IServiceCollection services,
            Action<RepositoryOptions> setupAction = default,
            Action<CosmosClientOptions> additionSetupAction = default)
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
                    .AddSingleton(new CosmosClientOptionsManipulator(additionSetupAction))
                    .AddSingleton<ICosmosClientOptionsProvider, DefaultCosmosClientOptionsProvider>()
                    .AddSingleton<ICosmosClientProvider, DefaultCosmosClientProvider>()
                    .AddSingleton(typeof(ICosmosContainerProvider<>), typeof(DefaultCosmosContainerProvider<>))
                    .AddSingleton<ICosmosPartitionKeyPathProvider, DefaultCosmosPartitionKeyPathProvider>()
                    .AddSingleton<ICosmosContainerNameProvider, DefaultCosmosContainerNameProvider>()
                    .AddSingleton<ICosmosUniqueKeyPolicyProvider, DefaultCosmosUniqueKeyPolicyProvider>()
                    .AddSingleton(typeof(IRepository<>), typeof(DefaultRepository<>))
                    .AddSingleton<IRepositoryFactory, DefaultRepositoryFactory>()
                    .AddSingleton<ICosmosItemConfigurationProvider, DefaultCosmosItemConfigurationProvider>()
                    .AddSingleton<IRepositoryOptionsValidator, DefaultRepositoryOptionsValidator>();

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

            services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>))
                .AddSingleton<IRepositoryFactory, DefaultRepositoryFactory>();

            return services;
        }

        /// <summary>
        /// Remove all of <see cref="IRepository{TItem}"/> from the container.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns></returns>
        public static IServiceCollection RemoveCosmosRepositories(this IServiceCollection services)
        {
            List<ServiceDescriptor> repositories = services.Where(i => i.ServiceType == typeof(IRepository<>)).ToList();
            repositories.ForEach(r => services.Remove(r));
            return services;
        }

        /// <summary>
        /// Adds the services required to consume any number of <see cref="IRepository{TItem}"/>
        /// instances to interact with Cosmos DB.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The configuration representing the applications settings.</param>
        /// <param name="setupAction">An action to configure the repository options</param>
        /// <returns>The same service collection that was provided, with the required cosmos services.</returns>
        [Obsolete(
            "Use the AddCosmosRepository overload the doesn't accept an IConfiguration. " +
            "This is no longer needed, and will be removed in later versions.")]
        public static IServiceCollection AddCosmosRepository(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<RepositoryOptions> setupAction = default) =>
            services.AddCosmosRepository(setupAction);
    }
}
