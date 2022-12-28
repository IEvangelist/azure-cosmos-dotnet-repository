// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Attributes;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcingTests;

public static class Testing
{
    public record SampleEvent(string FirstProp = "A", string SecondProp = "B") : DomainEvent;

    public class SampleEventItem : EventItem
    {
        public SampleEventItem(
            DomainEvent domainEvent,
            string partitionKey)
        {
            DomainEvent = domainEvent;
            PartitionKey = partitionKey;
        }
    }

    public class TestAggregateWithNoPk : AggregateRoot
    {
        public string FirstProp { get; private set; } = null!;
        public string SecondProp { get; private set; } = null!;

        public void SetEvents(IReadOnlyList<DomainEvent> domainEvents)
        {
            foreach (DomainEvent domainEvent in domainEvents)
            {
                AddEvent(domainEvent);
            }
        }

        protected override void Apply(DomainEvent domainEvent)
        {
            if (domainEvent is SampleEvent sampleEvent)
            {
                FirstProp = sampleEvent.FirstProp;
                SecondProp = sampleEvent.SecondProp;
            }
        }
    }

    public class TestAggregateWithSinglePk : AggregateRoot
    {
        [EventItemPartitionKey]
        public string FirstProp { get; private set; } = null!;

        public string SecondProp { get; private set; } = null!;

        public void SetEvents(IReadOnlyList<DomainEvent> domainEvents)
        {
            foreach (DomainEvent domainEvent in domainEvents)
            {
                AddEvent(domainEvent);
            }
        }

        public new void AddEvent(DomainEvent domainEvent) => base.AddEvent(domainEvent);

        protected override void Apply(DomainEvent domainEvent)
        {
            if (domainEvent is Testing.SampleEvent sampleEvent)
            {
                FirstProp = sampleEvent.FirstProp;
                SecondProp = sampleEvent.SecondProp;
            }
        }
    }

    public class TestAggregateWithMultiplePk : AggregateRoot
    {
        [EventItemPartitionKey]
        public string FirstProp { get; private set; } = null!;

        [EventItemPartitionKey]
        public string SecondProp { get; private set; } = null!;

        public void SetEvents(IReadOnlyList<SampleEvent> domainEvents)
        {
            foreach (SampleEvent domainEvent in domainEvents)
            {
                AddEvent(domainEvent);
            }
        }

        protected override void Apply(DomainEvent domainEvent)
        {
            if (domainEvent is Testing.SampleEvent sampleEvent)
            {
                FirstProp = sampleEvent.FirstProp;
                SecondProp = sampleEvent.SecondProp;
            }
        }
    }
}