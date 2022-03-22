// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;

namespace Microsoft.Azure.CosmosEventSourcing.Builders;

/// <summary>
/// Provides a mechanism to extend the implementation of the an <see cref="IEventItemProjectionBuilder{TEventItem,TProjectionKey}"/> via provided decorators
/// </summary>
public interface IEventItemProjectionBuilderDecorators<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey
{
    /// <summary>
    /// Access to the <see cref="ICosmosEventSourcingBuilder"/>
    /// </summary>
    ICosmosEventSourcingBuilder EventSourcingBuilder { get; }

    /// <summary>
    /// Decorators an instance of an <see cref="IEventItemProjectionBuilder{TEventItem,TProjectionKey}"/> with an implementation that will catch any exceptions and write them
    /// to a dead letter container.
    /// </summary>
    IEventItemProjectionBuilderDecorators<TEventItem, TProjectionKey> WithDeadLetterDecorator(
        Action<DeadLetterOptions<TEventItem, TProjectionKey>>? optionsAction = null);
}