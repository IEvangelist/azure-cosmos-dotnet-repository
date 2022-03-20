// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosEventSourcing.Builders;

internal class DefaultCosmosEventSourcingBuilder : ICosmosEventSourcingBuilder
{
    private readonly IServiceCollection _services;

    public DefaultCosmosEventSourcingBuilder(IServiceCollection services) =>
        _services = services;

    public ICosmosEventSourcingBuilder AddEventItemProjectionBuilder<TEventItem,TProjectionKey, TProjectionBuilder>(
        Action<EventSourcingProcessorOptions<TEventItem>>? optionsAction = null)
        where TEventItem : EventItem
        where TProjectionBuilder : class, IEventItemProjectionBuilder<TEventItem, TProjectionKey>
        where TProjectionKey : IProjectionKey
    {
        EventSourcingProcessorOptions<TEventItem> options = new();
        optionsAction?.Invoke(options);

        _services.AddSingleton(options);
        _services.AddSingleton<IEventItemProjectionBuilder<TEventItem, TProjectionKey>, TProjectionBuilder>();
        _services.AddSingleton<IEventSourcingProcessor, DefaultEventSourcingProcessor<TEventItem, TProjectionKey>>();
        return this;
    }


    public ICosmosEventSourcingBuilder AddDefaultDomainEventProjectionBuilder<TEventItem, TProjectionKey>(
        Action<EventSourcingProcessorOptions<TEventItem>>? optionsAction = null)
        where TEventItem : EventItem where TProjectionKey : IProjectionKey
    {
        EventSourcingProcessorOptions<TEventItem> options = new();
        optionsAction?.Invoke(options);

        _services.AddSingleton(options);
        _services
            .AddSingleton<IEventItemProjectionBuilder<TEventItem, TProjectionKey>, DefaultDomainEventProjectionBuilder<TEventItem, TProjectionKey>>();
        _services.AddSingleton<IEventSourcingProcessor, DefaultEventSourcingProcessor<TEventItem, TProjectionKey>>();
        return this;
    }

    public ICosmosEventSourcingBuilder AddDomainEventTypes(
        params Assembly[] assemblies)
    {
        if (!assemblies.Any())
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        List<Type> types = assemblies
            .SelectMany(x => x.GetTypes()
                .Where(type => typeof(IDomainEvent).IsAssignableFrom(type)))
            .ToList();

        types.ForEach(t => DomainEventConverter.ConvertableTypes.Add(t));

        return this;
    }

    public ICosmosEventSourcingBuilder AddDomainEventProjectionHandlers(
        params Assembly[] assemblies)
    {
        if (!assemblies.Any())
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        _services.Scan(x => x.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventProjectionBuilder<,,>)))
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