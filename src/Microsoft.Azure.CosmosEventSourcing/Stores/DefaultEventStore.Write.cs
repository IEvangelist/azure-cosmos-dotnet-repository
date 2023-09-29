// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using System.Runtime.CompilerServices.Context;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Attributes;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

internal partial class DefaultEventStore<TEventItem>
{
    public async ValueTask PersistAsync(
        IEnumerable<TEventItem> items,
        CancellationToken cancellationToken = default)
    {
        var eventItems = items.ToList();
        if (eventItems is { Count: 0 })
        {
            return;
        }

        if (eventItems.Count(x => x.EventName is nameof(AtomicEvent)) is not 1)
        {
            throw new AtomicEventRequiredException();
        }

        await batchRepository.UpdateAsBatchAsync(
            SetCorrelationId(contextService, eventItems),
            cancellationToken);
    }

    public async ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default) =>
            await PersistAsync(
                aggregateRoot,
                GetEventItemPartitionKeyValue(aggregateRoot),
                cancellationToken);

    public ValueTask PersistAsync<TAggregateRoot>(
        TAggregateRoot aggregateRoot,
        IAggregateRootMapper<TAggregateRoot, TEventItem> mapper,
        CancellationToken cancellationToken = default)
        where TAggregateRoot : IAggregateRoot =>
        batchRepository.UpdateAsBatchAsync(
            SetCorrelationId(contextService, mapper.MapFrom(aggregateRoot)),
            cancellationToken);

    public async ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) =>
            await PersistAsync(
                SetCorrelationId(contextService, BuildEvents(aggregateRoot, partitionKeyValue)),
                cancellationToken);

    private static IEnumerable<TEventItem> SetCorrelationId(
        IContextService contextService,
        IEnumerable<TEventItem> eventItems)
    {
        if (string.IsNullOrWhiteSpace(contextService.CorrelationId))
        {
            return eventItems;
        }

        IEnumerable<TEventItem> items = eventItems.ToList();
        foreach (TEventItem item in items)
        {
            if (item is not null && item.DomainEvent is not AtomicEvent)
            {
                item.CorrelationId = contextService.CorrelationId;
            }
        }

        return items;
    }

    private static IEnumerable<TEventItem> BuildEvents(
        IAggregateRoot aggregateRoot,
        string partitionKey)
    {
        var events = aggregateRoot.NewEvents
            .Select(x =>
                Activator.CreateInstance(
                    typeof(TEventItem),
                    x,
                    partitionKey) as TEventItem)
            .ToList();

        events.Add(Activator.CreateInstance(
            typeof(TEventItem),
            aggregateRoot.AtomicEvent,
            partitionKey) as TEventItem);

        return events.Any(x => x is null)
            ? throw new InvalidOperationException(
                $"At least one of the {typeof(TEventItem).Name} could not be constructed")
            : (IEnumerable<TEventItem>)events;
    }

    private static string GetEventItemPartitionKeyValue<TAggregate>(TAggregate aggregate)
        where TAggregate : IAggregateRoot
    {
        var partitionKeyProperties = aggregate
            .GetType()
            .GetProperties()
            .Where(x
                => x.GetCustomAttributes()
                    .Any(y =>
                        y is EventItemPartitionKeyAttribute))
            .ToList();

        switch (partitionKeyProperties.Count)
        {
            case 0:
                throw new EventItemPartitionKeyAttributeRequiredException(aggregate.GetType());
            case > 1:
                throw new InvalidEventItemPartitionKeyAttributeCombinationException(aggregate.GetType());
        }

        PropertyInfo partitionKeyProperty = partitionKeyProperties.Single();
        var partitionKey = partitionKeyProperty.GetValue(aggregate) ??
            throw new InvalidPartitionKeyValueException(
                partitionKeyProperty.Name,
                aggregate.GetType());

        return partitionKey?.ToString() ??
            throw new InvalidPartitionKeyValueException(
                partitionKeyProperty.Name,
                aggregate.GetType());
    }
}