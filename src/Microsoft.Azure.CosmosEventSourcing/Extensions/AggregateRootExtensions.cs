// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Attributes;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

public static class AggregateRootExtensions
{
    public static IEnumerable<TEventItem> ToEventItems<TEventItem>(
        this IAggregateRoot aggregateRoot,
        string partitionKey,
        bool isSequenceNumberingDisabled)
    where TEventItem : EventItem
    {
        var events = aggregateRoot.NewEvents
            .Select(x =>
                Activator.CreateInstance(
                    typeof(TEventItem),
                    x,
                    partitionKey) as TEventItem)
            .ToList();

        if(isSequenceNumberingDisabled is false)
        {
            events.Add(Activator.CreateInstance(
                typeof(TEventItem),
                aggregateRoot.AtomicEvent,
                partitionKey) as TEventItem);
        }

        return events.Any(x => x is null)
            ? throw new InvalidOperationException(
                $"At least one of the {typeof(TEventItem).Name} could not be constructed")
            : (IEnumerable<TEventItem>)events;
    }

    public static string GetEventItemPartitionKeyValue<TAggregate>(this TAggregate aggregate)
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