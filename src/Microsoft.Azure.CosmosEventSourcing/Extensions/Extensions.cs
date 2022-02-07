// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using System.Security.Policy;
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

    public static IServiceCollection AddCosmosEventSourcing(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddSingleton(typeof(IEventSourceRepository<>), typeof(EventSourceRepository<>));
        services.AddSingleton<IChangeFeedContainerProcessorProvider, EventSourcingProvider>();
        services.AddAllPersistedEventsTypes(assemblies);
        return services;
    }

    public static IServiceCollection AddEventSourceProjectionBuilder<TEventSource, TProjectionBuilder>(
        this IServiceCollection services,
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource
        where TProjectionBuilder : class, ISourceProjectionBuilder<TEventSource>
    {
        EventSourcingProcessorOptions<TEventSource> options = new();
        optionsAction?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<ISourceProjectionBuilder<TEventSource>, TProjectionBuilder>();
        services.AddSingleton<IEventSourcingProcessor, EventSourcingProcessor<TEventSource>>();
        return services;
    }

    public static IServiceCollection AddEventSourceProjectionBuilder<TEventSource>(
        this IServiceCollection services,
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource
    {
        EventSourcingProcessorOptions<TEventSource> options = new();
        optionsAction?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<ISourceProjectionBuilder<TEventSource>, EventBasedSourceProjectionBuilder<TEventSource>>();
        services.AddSingleton<IEventSourcingProcessor, EventSourcingProcessor<TEventSource>>();
        return services;
    }

    public static IServiceCollection AddAllPersistedEventsTypes(
        this IServiceCollection services,
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

        types.ForEach(PersistedEventConverter.ConvertableTypes.Add);

        return services;
    }

    public static IServiceCollection AddAllEventProjectionHandlers(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (!assemblies.Any())
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        services.Scan(x => x.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IEventProjectionHandler<>)))
            .AsImplementedInterfaces().WithSingletonLifetime());

        return services;
    }
}