// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;

namespace Microsoft.Azure.CosmosEventSourcing.Builders;

/// <summary>
/// Provides a mechanism to extend the implementation of the an <see cref="IEventItemProjection{TEventItem,TProjectionKey}"/> via provided decorators
/// </summary>
public interface IEventItemProjectionBuilder<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey
{
    /// <summary>
    /// Access to the <see cref="ICosmosEventSourcingBuilder"/>
    /// </summary>
    ICosmosEventSourcingBuilder EventSourcingBuilder { get; }

    /// <summary>
    /// Decorators an instance of an <see cref="IEventItemProjection{TEventItem,TProjectionKey}"/> with an implementation that will catch any exceptions and write them
    /// to a dead letter container.
    /// </summary>
    IEventItemProjectionBuilder<TEventItem, TProjectionKey> WithDeadLetterDecorator(
        Action<DeadLetterOptions<TEventItem, TProjectionKey>>? optionsAction = null);
}