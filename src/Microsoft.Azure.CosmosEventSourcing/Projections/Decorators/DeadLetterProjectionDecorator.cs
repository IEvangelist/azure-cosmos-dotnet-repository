// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Models;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.Projections.Decorators;

internal class DeadLetterProjectionDecorator<TEventItem, TProjectionKey>(
    ILogger<DeadLetterProjectionDecorator<TEventItem, TProjectionKey>> logger,
    EventSourcingProcessorOptions<TEventItem, TProjectionKey> processorOptions,
    IEventItemProjection<TEventItem, TProjectionKey> inner,
    DeadLetterOptions<TEventItem, TProjectionKey> options,
    IWriteOnlyRepository<DeadLetteredEventItem<TEventItem>> repository) :
    IEventItemProjection<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey

{
    public async ValueTask ProjectAsync(TEventItem eventItem, CancellationToken cancellationToken = default)
    {
        try
        {
            await inner.ProjectAsync(eventItem, cancellationToken);
        }
        catch (Exception e)
        {
            await HandleFailedProjectionAsync(eventItem, e);
        }
    }

    private async Task HandleFailedProjectionAsync(TEventItem eventItem, Exception exception)
    {
        // Avoid Error CS9113  Parameter 'options' is unread.
        _ = options;

        logger.LogWarning("Writing dead lettered event item for event {EventId} with name {EventName}",
            eventItem.Id,
            eventItem.EventName);

        DeadLetteredEventItem<TEventItem> deadLetteredItem = new(
            eventItem,
            processorOptions.ProcessorName,
            processorOptions.InstanceName,
            typeof(TProjectionKey).Name,
            new ExceptionDetails(
                exception.GetType().Name,
                exception.ToString()));

        try
        {
            await repository.CreateAsync(deadLetteredItem);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to save dead lettered event item for event {EventId} with name {EventName}",
                eventItem.Id,
                eventItem.EventName);
            throw;
        }
    }
}