// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

public static class Extensions
{
    public static IItemContainerBuilder ConfigureEventSourceStore<TSourcedEvent>(
        this IItemContainerBuilder containerBuilder,
        string containerName)
        where TSourcedEvent : EventSource
    {
        containerBuilder.Configure<TSourcedEvent>(options =>
        {
            options.WithContainer(containerName);
            options.WithPartitionKey("/partitionKey");
        });

        return containerBuilder;
    }

    public static IServiceCollection AddCosmosEventSourcing(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IEventSourceRepository<>), typeof(EventSourceRepository<>));
        services.AddSingleton<IChangeFeedContainerProcessorProvider, EventSourcingProvider>();
        services.AddAllPersistedEvents();
        return services;
    }

    public static IServiceCollection AddEventSourceProcessing<TEventSource, TProjectionBuilder>(
        this IServiceCollection services,
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource
        where TProjectionBuilder : class, ISourceProjectionBuilder<TEventSource>
    {
        EventSourcingProcessorOptions<TEventSource> options = new();
        optionsAction?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<ISourceProjectionBuilder<TEventSource>, TProjectionBuilder>();
        services.AddSingleton<IContainerChangeFeedProcessor, EventSourcingProcessor<TEventSource>>();
        return services;
    }

    private static IServiceCollection AddAllPersistedEvents(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (!assemblies.Any())
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        List<Type> types = assemblies
            .SelectMany(x => x.GetTypes()
                .Where(type => type.IsAssignableTo(typeof(IPersistedEvent))))
            .ToList();

        types.ForEach(x => PersistedEventConverter.ConvertableTypes.Add(x));

        return services;
    }
}