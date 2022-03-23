// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Models;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.Projections.Decorators;

internal class DeadLetterProjectionDecorator<TEventItem, TProjectionKey> :
    IEventItemProjection<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey

{
    private readonly ILogger<DeadLetterProjectionDecorator<TEventItem, TProjectionKey>> _logger;
    private readonly EventSourcingProcessorOptions<TEventItem, TProjectionKey> _processorOptions;
    private readonly IEventItemProjection<TEventItem, TProjectionKey> _inner;
    private readonly DeadLetterOptions<TEventItem, TProjectionKey> _options;
    private readonly IWriteOnlyRepository<DeadLetteredEventItem<TEventItem>> _repository;

    public DeadLetterProjectionDecorator(
        ILogger<DeadLetterProjectionDecorator<TEventItem, TProjectionKey>> logger,
        EventSourcingProcessorOptions<TEventItem, TProjectionKey> processorOptions,
        IEventItemProjection<TEventItem, TProjectionKey> inner,
        DeadLetterOptions<TEventItem, TProjectionKey> options,
        IWriteOnlyRepository<DeadLetteredEventItem<TEventItem>> repository)
    {
        _logger = logger;
        _processorOptions = processorOptions;
        _inner = inner;
        _options = options;
        _repository = repository;
    }

    public async ValueTask ProjectAsync(TEventItem eventItem, CancellationToken cancellationToken = default)
    {
        try
        {
            await _inner.ProjectAsync(eventItem, cancellationToken);
        }
        catch (Exception e)
        {
            await HandleFailedProjectionAsync(eventItem, e);
        }
    }

    private async Task HandleFailedProjectionAsync(TEventItem eventItem, Exception exception)
    {
        _logger.LogWarning("Writing dead lettered event item for event {EventId} with name {EventName}",
            eventItem.Id,
            eventItem.EventName);

        DeadLetteredEventItem<TEventItem> deadLetteredItem = new(
            eventItem,
            _processorOptions.ProcessorName,
            _processorOptions.InstanceName,
            typeof(TProjectionKey).Name,
            new ExceptionDetails(
                exception.GetType().Name,
                exception.ToString()));

        try
        {
            await _repository.CreateAsync(deadLetteredItem);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save dead lettered event item for event {EventId} with name {EventName}",
                eventItem.Id,
                eventItem.EventName);
            throw;
        }
    }
}