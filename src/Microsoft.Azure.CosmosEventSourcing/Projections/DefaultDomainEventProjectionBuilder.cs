// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

internal class DefaultDomainEventProjectionBuilder<TEventItem, TProjectionKey> : IEventItemProjectionBuilder<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey
{
    private readonly ILogger<DefaultDomainEventProjectionBuilder<TEventItem, TProjectionKey>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DefaultDomainEventProjectionBuilder(
        ILogger<DefaultDomainEventProjectionBuilder<TEventItem, TProjectionKey>> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask ProjectAsync(TEventItem eventItem, CancellationToken cancellationToken = default)
    {
        string payloadTypeName = eventItem.DomainEvent.GetType().Name;
        Type handlerType = BuildEventProjectionHandlerType(eventItem);
        IEnumerable<object?> handlers = _serviceProvider.GetServices(handlerType).ToList();

        if (handlers.Any() is false)
        {
            if (payloadTypeName is not nameof(AtomicEvent))
            {
                _logger.LogDebug("No IDomainEventProjectionBuilder<{EventType}> found",
                    payloadTypeName);
            }

            return;
        }

        await Task.WhenAll(handlers.Select(handler =>
        {
            try
            {
                object? result = handlerType.GetMethod("HandleAsync")?
                    .Invoke(handler, new object[] {eventItem.DomainEvent, eventItem, cancellationToken});

                if (result is ValueTask valueTask)
                {
                    return valueTask.AsTask();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Failed when handling event {PersistedEventType} with {EventProjectionHandlerType}",
                    payloadTypeName, handler?.GetType().Name);
            }

            return Task.CompletedTask;
        }));
    }

    private static Type BuildEventProjectionHandlerType(TEventItem eventSource) =>
        typeof(IDomainEventProjectionBuilder<,,>).MakeGenericType(eventSource.DomainEvent.GetType(), eventSource.GetType(), typeof(TProjectionKey));
}