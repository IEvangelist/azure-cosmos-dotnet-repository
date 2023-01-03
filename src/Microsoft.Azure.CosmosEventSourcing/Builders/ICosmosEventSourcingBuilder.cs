// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosEventSourcing.Builders;

/// <summary>
/// A builder used to configure the event sourcing library.
/// </summary>
public interface ICosmosEventSourcingBuilder
{
    /// <summary>
    /// Adds a custom <see cref="IEventItemProjection{TEventItem,TProjectionKey}"/> to the library.
    /// </summary>
    /// <param name="optionsAction">The <see cref="EventSourcingProcessorOptions{TEventItem, TProjectionKey}"/> used to configure the processor.</param>
    /// <typeparam name="TEventItem">The <see cref="EventItem"/></typeparam>
    /// <typeparam name="TProjection">The custom type of <see cref="IEventItemProjection{TEventItem,TProjectionKey}"/></typeparam>
    /// <typeparam name="TProjectionKey">The key that identifies this projection.</typeparam>
    /// <returns></returns>
    public IEventItemProjectionBuilder<TEventItem, TProjectionKey> AddEventItemProjection<TEventItem, TProjectionKey, TProjection>(
        Action<EventSourcingProcessorOptions<TEventItem, TProjectionKey>>? optionsAction = null)
        where TEventItem : EventItem
        where TProjection : class, IEventItemProjection<TEventItem, TProjectionKey>
        where TProjectionKey : IProjectionKey;

    /// <summary>
    /// Adds a projection builder that uses <see cref="IDomainEventProjection{TEvent,TEventItem,TProjectionKey}"/>'s to project a single type of <see cref="IDomainEvent"/>
    /// </summary>
    /// <param name="optionsAction">The <see cref="EventSourcingProcessorOptions{TEventItem, TProjectionKey}"/> used to configure the processor.</param>
    /// <typeparam name="TEventItem">The <see cref="EventItem"/></typeparam>
    /// <typeparam name="TProjectionKey"></typeparam>
    /// <returns></returns>
    public ICosmosEventSourcingBuilder AddDefaultDomainEventProjection<TEventItem, TProjectionKey>(
        Action<EventSourcingProcessorOptions<TEventItem, TProjectionKey>>? optionsAction = null)
        where TEventItem : EventItem
        where TProjectionKey : IProjectionKey;

    /// <summary>
    /// Adds all <see cref="IDomainEvent"/> to the custom json converter.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for <see cref="IDomainEvent"/></param>
    /// <returns></returns>
    public ICosmosEventSourcingBuilder AddDomainEventTypes(
        params Assembly[] assemblies);

    /// <summary>
    /// Adds all of the <see cref="IDomainEventProjection{TEvent,TEventItem,TProjectionKey}"/>'s provided in the given assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for <see cref="IDomainEventProjection{TEvent,TEventItem,TProjectionKey}"/></param>
    /// <returns></returns>
    public ICosmosEventSourcingBuilder AddDomainEventProjectionHandlers(
        params Assembly[] assemblies);

    /// <summary>
    /// Adds the services required to consume any number of <see cref="IRepository{TItem}"/>
    /// instances to interact with Cosmos DB.
    /// </summary>
    /// <param name="setupAction">An action to configure the repository options</param>
    /// <param name="additionSetupAction">An action to configure the <see cref="CosmosClientOptions"/></param>
    /// <returns>The same service collection that was provided, with the required cosmos services.</returns>
    public ICosmosEventSourcingBuilder AddCosmosRepository(
        Action<RepositoryOptions>? setupAction = default,
        Action<CosmosClientOptions>? additionSetupAction = default);
}