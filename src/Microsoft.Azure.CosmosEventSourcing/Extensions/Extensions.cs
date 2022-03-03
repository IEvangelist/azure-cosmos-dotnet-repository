// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.Builders;
using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

public static class Extensions
{
    public static IItemContainerBuilder ConfigureEventSourceStore<TSourcedEvent>(
        this IItemContainerBuilder containerBuilder,
        string containerName,
        Action<ContainerOptionsBuilder>? containerOptionsBuilder = null)
        where TSourcedEvent : EventSource
    {
        containerBuilder.Configure<TSourcedEvent>(options =>
        {
            options.WithContainer(containerName);
            options.WithPartitionKey("/partitionKey");
            containerOptionsBuilder?.Invoke(options);
        });

        return containerBuilder;
    }

    public static IItemContainerBuilder ConfigureProjectionStore<TProjection>(
        this IItemContainerBuilder containerBuilder,
        string containerName,
        string partitionKey = "/partitionKey",
        Action<ContainerOptionsBuilder>? builder = default)
        where TProjection : IItem
    {
        containerBuilder.Configure<TProjection>(options =>
        {
            options.WithContainer(containerName);
            options.WithPartitionKey(partitionKey);
            builder?.Invoke(options);
        });

        return containerBuilder;
    }

    public static IServiceCollection AddCosmosEventSourcing(
        this IServiceCollection services,
        Action<ICosmosEventSourcingBuilder> eventSourcingBuilder,
        params Assembly[] assemblies)
    {
        CosmosEventSourcingBuilder builder = new(services);
        eventSourcingBuilder.Invoke(builder);
        services.AddSingleton(typeof(IEventSourceRepository<>), typeof(EventSourceRepository<>));
        services.AddSingleton<IChangeFeedContainerProcessorProvider, EventSourcingProvider>();
        return services;
    }


}