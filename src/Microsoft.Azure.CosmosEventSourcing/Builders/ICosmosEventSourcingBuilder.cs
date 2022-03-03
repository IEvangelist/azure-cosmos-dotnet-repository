// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.Projections;

namespace Microsoft.Azure.CosmosEventSourcing.Builders;

public interface ICosmosEventSourcingBuilder
{
    public ICosmosEventSourcingBuilder AddEventSourceProjectionBuilder<TEventSource, TProjectionBuilder>(
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource
        where TProjectionBuilder : class, ISourceProjectionBuilder<TEventSource>;

    public ICosmosEventSourcingBuilder AddEventSourceProjectionBuilder<TEventSource>(
        Action<EventSourcingProcessorOptions<TEventSource>>? optionsAction = null)
        where TEventSource : EventSource;

    public ICosmosEventSourcingBuilder AddAllPersistedEventsTypes(
        params Assembly[] assemblies);

    public ICosmosEventSourcingBuilder AddAllEventProjectionHandlers(
        params Assembly[] assemblies);
}