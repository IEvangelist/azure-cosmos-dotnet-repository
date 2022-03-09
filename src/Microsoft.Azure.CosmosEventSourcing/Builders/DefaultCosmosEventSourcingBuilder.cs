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

internal class DefaultCosmosEventSourcingBuilder : ICosmosEventSourcingBuilder
{
    private readonly IServiceCollection _services;

    public DefaultCosmosEventSourcingBuilder(IServiceCollection services) =>
        _services = services;

    public ICosmosEventSourcingBuilder AddEventItemProjectionBuilder<TEventItem, TProjectionBuilder>(
        Action<EventSourcingProcessorOptions<TEventItem>>? optionsAction = null)
        where TEventItem : EventItem
        where TProjectionBuilder : class, IEventItemProjectionBuilder<TEventItem>
    {
        EventSourcingProcessorOptions<TEventItem> options = new();
        optionsAction?.Invoke(options);

        _services.AddSingleton(options);
        _services.AddSingleton<IEventItemProjectionBuilder<TEventItem>, TProjectionBuilder>();
        _services.AddSingleton<IEventSourcingProcessor, DefaultEventSourcingProcessor<TEventItem>>();
        return this;
    }

    public ICosmosEventSourcingBuilder AddEventItemProjectionBuilder<TEventItem>(
        Action<EventSourcingProcessorOptions<TEventItem>>? optionsAction = null)
        where TEventItem : EventItem
    {
        EventSourcingProcessorOptions<TEventItem> options = new();
        optionsAction?.Invoke(options);

        _services.AddSingleton(options);
        _services
            .AddSingleton<IEventItemProjectionBuilder<TEventItem>, DefaultDomainEventProjectionBuilder<TEventItem>>();
        _services.AddSingleton<IEventSourcingProcessor, DefaultEventSourcingProcessor<TEventItem>>();
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

        types.ForEach(t => PersistedEventConverter.ConvertableTypes.Add(t));

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
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventProjectionBuilder<,>)))
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