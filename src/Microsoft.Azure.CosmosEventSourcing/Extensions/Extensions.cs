// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.Builders;
using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
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

    public static IServiceCollection AddCosmosEventSourcing(
        this IServiceCollection services,
        Action<ICosmosEventSourcingBuilder> builder,
        params Assembly[] assemblies)
    {
        services.AddSingleton(typeof(IEventSourceRepository<>), typeof(EventSourceRepository<>));
        services.AddSingleton<IChangeFeedContainerProcessorProvider, EventSourcingProvider>();
        return services;
    }
}