// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

internal class EventBasedEventSourceProjectionBuilder<TEventSource> : IEventSourceProjectionBuilder<TEventSource>
    where TEventSource : EventSource
{
    private readonly ILogger<EventBasedEventSourceProjectionBuilder<TEventSource>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EventBasedEventSourceProjectionBuilder(
        ILogger<EventBasedEventSourceProjectionBuilder<TEventSource>> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask ProjectAsync(TEventSource sourcedEvent, CancellationToken cancellationToken = default)
    {
        string payloadTypeName = sourcedEvent.EventPayload.GetType().Name;
        Type handlerType = BuildEventProjectionHandlerType(sourcedEvent);
        IEnumerable<object?> handlers = _serviceProvider.GetServices(handlerType).ToList();

        if (handlers.Any() is false)
        {
            _logger.LogDebug("No IEventProjectionHandler<{EventType}> found",
                payloadTypeName);

            return;
        }

        await Task.WhenAll(handlers.Select(handler =>
        {
            try
            {
                object? result = handlerType.GetMethod("HandleAsync")?
                    .Invoke(handler, new object[] {sourcedEvent.EventPayload, sourcedEvent, cancellationToken});

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

    private static Type BuildEventProjectionHandlerType(TEventSource eventSource) =>
        typeof(IEventProjectionHandler<,>).MakeGenericType(eventSource.EventPayload.GetType(), eventSource.GetType());
}