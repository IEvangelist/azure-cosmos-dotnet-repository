// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Builders;
using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

/// <summary>
/// The extensions used to add the library to an <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the event sourcing library to the <see cref="IServiceCollection"/> container.
    /// </summary>
    /// <param name="services">The collection used to add the services for the cosmos event sourcing library.</param>
    /// <param name="eventSourcingBuilder">A <see cref="ICosmosEventSourcingBuilder"/> used to configure the library.</param>
    /// <returns>The instance of the <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCosmosEventSourcing(
        this IServiceCollection services,
        Action<ICosmosEventSourcingBuilder> eventSourcingBuilder)
    {
        DefaultCosmosEventSourcingBuilder builder = new(services);
        eventSourcingBuilder.Invoke(builder);
        services.AddSingleton(typeof(IEventStore<>), typeof(DefaultEventStore<>));
        services.AddSingleton<IChangeFeedContainerProcessorProvider, DefaultEventSourcingProvider>();
        return services;
    }
}