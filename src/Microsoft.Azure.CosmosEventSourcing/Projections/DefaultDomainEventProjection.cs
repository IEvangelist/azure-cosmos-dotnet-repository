// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

internal class
    DefaultDomainEventProjection<TEventItem, TProjectionKey>(
    ILogger<DefaultDomainEventProjection<TEventItem, TProjectionKey>> logger,
    IServiceProvider serviceProvider) : IEventItemProjection<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey
{
    public async ValueTask ProjectAsync(TEventItem eventItem, CancellationToken cancellationToken = default)
    {
        var payloadTypeName = eventItem.DomainEvent.GetType().Name;
        Type handlerType = BuildEventProjectionHandlerType(eventItem);
        IEnumerable<object?> handlers = serviceProvider.GetServices(handlerType).ToList();

        if (handlers.Any() is false)
        {
            if (eventItem.DomainEvent is NonDeserializableEvent nonDeserializableEvent)
            {
                logger.LogError(
                    "The event with name {EventName} could not be deserialized as it was not registered with the custom deserializer payload = {EventPayload}",
                    nonDeserializableEvent.Name,
                    nonDeserializableEvent.Payload.ToString());
                return;
            }

            if (payloadTypeName is not nameof(AtomicEvent))
            {
                logger.LogDebug("No IDomainEventProjection<{EventType}> found",
                    payloadTypeName);
            }

            return;
        }

        await Task.WhenAll(handlers.Select(handler =>
        {
            try
            {
                var result = handlerType.GetMethod("HandleAsync")?
                    .Invoke(handler, [eventItem.DomainEvent, eventItem, cancellationToken]);

                if (result is ValueTask valueTask)
                {
                    return valueTask.AsTask();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    "Failed when handling event {PersistedEventType} with {EventProjectionHandlerType}",
                    payloadTypeName, handler?.GetType().Name);
            }

            return Task.CompletedTask;
        }));
    }

    private static Type BuildEventProjectionHandlerType(TEventItem eventSource) =>
        typeof(IDomainEventProjection<,,>).MakeGenericType(eventSource.DomainEvent.GetType(), eventSource.GetType(),
            typeof(TProjectionKey));
}