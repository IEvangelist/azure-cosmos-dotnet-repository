// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosEventSourcing.Projections.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosEventSourcing.Builders;

internal class EventItemProjectionBuilder<TEventItem, TProjectionKey>(
    IServiceCollection services,
    ICosmosEventSourcingBuilder eventSourcingBuilder) :
    IEventItemProjectionBuilder<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey
{
    public ICosmosEventSourcingBuilder EventSourcingBuilder { get; } = eventSourcingBuilder;
    public IEventItemProjectionBuilder<TEventItem, TProjectionKey> WithDeadLetterDecorator(
        Action<DeadLetterOptions<TEventItem, TProjectionKey>>? optionsAction = null)
    {
        services.Decorate<IEventItemProjection<TEventItem, TProjectionKey>,
                DeadLetterProjectionDecorator<TEventItem, TProjectionKey>>();

        DeadLetterOptions<TEventItem, TProjectionKey> options = new();
        optionsAction?.Invoke(options);
        services.AddSingleton(options);

        return this;
    }
}