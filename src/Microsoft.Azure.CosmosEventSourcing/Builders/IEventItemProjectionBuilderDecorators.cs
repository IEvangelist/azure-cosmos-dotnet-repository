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
    ///
    /// </summary>
    /// <returns></returns>
    IEventItemProjectionBuilderDecorators<TEventItem, TProjectionKey> WithDeadLetterDecorator(
        Action<DeadLetterOptions<TEventItem, TProjectionKey>>? optionsAction = null);
}