// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
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
        List<TEventItem> eventItems = items.ToList();
        if (eventItems is { Count: 0 })
        {
            return;
        }

        if (eventItems.Count(x => x.EventName is nameof(AtomicEvent)) is not 1)
        {
            throw new AtomicEventRequiredException();
        }

        await _repository.UpdateAsBatchAsync(
            eventItems,
            cancellationToken);
    }

    public async ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default) =>
            await PersistAsync(
                aggregateRoot,
                GetEventItemPartitionKeyValue(aggregateRoot),
                cancellationToken);

    public async ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) =>
            await PersistAsync(
                BuildEvents(aggregateRoot, partitionKeyValue),
                cancellationToken);

    private static List<TEventItem> BuildEvents(IAggregateRoot aggregateRoot, string partitionKey)
    {
        List<TEventItem?> events = aggregateRoot.NewEvents
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

        if (events.Any(x => x == null))
        {
            throw new InvalidOperationException(
                $"At least one of the {typeof(TEventItem).Name} could not be constructed");
        }

        return events!;
    }

    private static string GetEventItemPartitionKeyValue<TAggregate>(TAggregate aggregate)
        where TAggregate : IAggregateRoot
    {
        List<PropertyInfo> partitionKeyProperties = aggregate
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
        Object partitionKey = partitionKeyProperty.GetValue(aggregate) ??
                              throw new InvalidPartitionKeyValueException(
                                  partitionKeyProperty.Name,
                                  aggregate.GetType());

        return partitionKey.ToString();
    }
}