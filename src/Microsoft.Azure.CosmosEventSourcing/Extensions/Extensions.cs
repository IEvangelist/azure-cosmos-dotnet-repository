// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

public static class Extensions
{
    public static IItemContainerBuilder ConfigureEventSourcingContainer<TSourcedEvent>(
        this IItemContainerBuilder containerBuilder,
        string containerName)
        where TSourcedEvent : SourcedEvent
    {
        containerBuilder.Configure<TSourcedEvent>(options =>
        {
            options.WithContainer(containerName);
            options.WithPartitionKey("/partitionKey");
        });

        return containerBuilder;
    }

    public static IServiceCollection AddCosmosEventStreaming(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IEventSourcingRepository<>), typeof(EventSourcingRepository<>));
        services.AddSingleton<IChangeFeedContainerProcessorProvider, EventSourcingProvider>();
        return services;
    }

    public static IServiceCollection AddEventSourcingContainerProcessing<TSourcedEvent>(this IServiceCollection services) where TSourcedEvent : SourcedEvent =>
        services.AddSingleton<IContainerChangeFeedProcessor, EventSourcingProcessor<TSourcedEvent>>();
}