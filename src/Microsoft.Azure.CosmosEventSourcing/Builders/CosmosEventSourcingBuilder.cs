// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosEventSourcing.Builders;

internal class CosmosEventSourcingBuilder : ICosmosEventSourcingBuilder
{
    private readonly IServiceCollection _services;

    public CosmosEventSourcingBuilder(IServiceCollection services) =>
        _services = services;

    public ICosmosEventSourcingBuilder AddEventSourceProjectionBuilder<TEventSource, TProjectionBuilder>(
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource
        where TProjectionBuilder : class, ISourceProjectionBuilder<TEventSource>
    {
        EventSourcingProcessorOptions<TEventSource> options = new();
        optionsAction?.Invoke(options);

        _services.AddSingleton(options);
        _services.AddSingleton<ISourceProjectionBuilder<TEventSource>, TProjectionBuilder>();
        _services.AddSingleton<IEventSourcingProcessor, EventSourcingProcessor<TEventSource>>();
        return this;
    }

    public ICosmosEventSourcingBuilder AddEventSourceProjectionBuilder<TEventSource>(
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource
    {
        EventSourcingProcessorOptions<TEventSource> options = new();
        optionsAction?.Invoke(options);

        _services.AddSingleton(options);
        _services.AddSingleton<ISourceProjectionBuilder<TEventSource>, EventBasedSourceProjectionBuilder<TEventSource>>();
        _services.AddSingleton<IEventSourcingProcessor, EventSourcingProcessor<TEventSource>>();
        return this;
    }

    public ICosmosEventSourcingBuilder AddAllPersistedEventsTypes(
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

        types.ForEach(t => PersistedEventConverter.ConvertableTypes.Add(t));

        return this;
    }

    public ICosmosEventSourcingBuilder AddAllEventProjectionHandlers(
        params Assembly[] assemblies)
    {
        if (!assemblies.Any())
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        _services.Scan(x => x.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IEventProjectionHandler<>)))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        return this;
    }
}