// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.Cosmos;
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
    /// Adds a custom <see cref="IEventSourceProjectionBuilder{TEventSource}"/> to the library.
    /// </summary>
    /// <param name="optionsAction">The <see cref="EventSourcingProcessorOptions{TEventSource}"/> used to configure the processor.</param>
    /// <typeparam name="TEventSource">The <see cref="EventSource"/></typeparam>
    /// <typeparam name="TProjectionBuilder">The custom type of <see cref="IEventSourceProjectionBuilder{TEventSource}"/></typeparam>
    /// <returns></returns>
    public ICosmosEventSourcingBuilder AddEventSourceProjectionBuilder<TEventSource, TProjectionBuilder>(
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource
        where TProjectionBuilder : class, IEventSourceProjectionBuilder<TEventSource>;

    /// <summary>
    /// Adds a projection builder that uses <see cref="IEventProjectionHandler{TEvent}"/>'s to project a single type of <see cref="IPersistedEvent"/>
    /// </summary>
    /// <param name="optionsAction">The <see cref="EventSourcingProcessorOptions{TEventSource}"/> used to configure the processor.</param>
    /// <typeparam name="TEventSource">The <see cref="EventSource"/></typeparam>
    /// <returns></returns>
    public ICosmosEventSourcingBuilder AddEventBasedEventSourceProjectionBuilder<TEventSource>(
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource;

    /// <summary>
    /// Adds all <see cref="IPersistedEvent"/> to the custom json converter.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for <see cref="IPersistedEvent"/></param>
    /// <returns></returns>
    public ICosmosEventSourcingBuilder AddAllPersistedEventsTypes(
        params Assembly[] assemblies);

    /// <summary>
    /// Adds all of the <see cref="IEventProjectionHandler{TEvent}"/>'s provided in the given assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for <see cref="IEventProjectionHandler{TEvent}"/></param>
    /// <returns></returns>
    public ICosmosEventSourcingBuilder AddAllEventProjectionHandlers(
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