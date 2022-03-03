// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository.Options;
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
        where TProjectionBuilder : class, IEventSourceProjectionBuilder<TEventSource>
    {
        EventSourcingProcessorOptions<TEventSource> options = new();
        optionsAction?.Invoke(options);

        _services.AddSingleton(options);
        _services.AddSingleton<IEventSourceProjectionBuilder<TEventSource>, TProjectionBuilder>();
        _services.AddSingleton<IEventSourcingProcessor, EventSourcingProcessor<TEventSource>>();
        return this;
    }

    public ICosmosEventSourcingBuilder AddEventBasedEventSourceProjectionBuilder<TEventSource>(
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource
    {
        EventSourcingProcessorOptions<TEventSource> options = new();
        optionsAction?.Invoke(options);

        _services.AddSingleton(options);
        _services
            .AddSingleton<IEventSourceProjectionBuilder<TEventSource>, EventBasedEventSourceProjectionBuilder<TEventSource>>();
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
                .Where(type => typeof(IPersistedEvent).IsAssignableFrom(type)))
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

    public ICosmosEventSourcingBuilder AddCosmosRepository(
        Action<RepositoryOptions>? setupAction = default,
        Action<CosmosClientOptions>? additionSetupAction = default)
    {
        _services.AddCosmosRepository(options =>
        {
            options.ContainerPerItemType = true;
            setupAction?.Invoke(options);
        }, additionSetupAction);

        return this;
    }
}