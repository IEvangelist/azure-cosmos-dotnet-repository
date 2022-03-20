// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosEventSourcing.Projections.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosEventSourcing.Builders;

internal class EventItemProjectionBuilderDecorator<TEventItem, TProjectionKey> :
    IEventItemProjectionBuilderDecorators<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey
{
    private readonly IServiceCollection _services;

    public EventItemProjectionBuilderDecorator(
        IServiceCollection services,
        ICosmosEventSourcingBuilder eventSourcingBuilder)
    {
        _services = services;
        EventSourcingBuilder = eventSourcingBuilder;
    }

    public ICosmosEventSourcingBuilder EventSourcingBuilder { get; }
    public IEventItemProjectionBuilderDecorators<TEventItem, TProjectionKey> WithDeadLetterDecorator(
        Action<DeadLetterOptions<TEventItem, TProjectionKey>>? optionsAction = null)
    {
        _services.Decorate<IEventItemProjectionBuilder<TEventItem, TProjectionKey>,
                DeadLetterProjectionBuilderDecorator<TEventItem, TProjectionKey>>();

        DeadLetterOptions<TEventItem, TProjectionKey> options = new();
        optionsAction?.Invoke(options);
        _services.AddSingleton(options);

        return this;
    }
}