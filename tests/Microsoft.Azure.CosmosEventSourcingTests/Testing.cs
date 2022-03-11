// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Attributes;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcingTests;

public static class Testing
{
    public record SampleEvent(string FirstProp = "A", string SecondProp = "B") : DomainEvent;
    public record GuidEvent(Guid Id) : DomainEvent;

    public class SampleEventItem : EventItem
    {
        public SampleEventItem(
            IDomainEvent eventPayload,
            string partitionKey) : base(eventPayload, partitionKey)
        {

        }

        public SampleEventItem(
            IDomainEvent eventPayload,
            Guid id) : base(eventPayload, id.ToString())
        {

        }
    }

    public class TestAggregateWithNoPk : AggregateRoot
    {
        public string FirstProp { get; private set; }
        public string SecondProp { get; private set; }

        public void SetEvents(IReadOnlyList<DomainEvent> domainEvents)
        {
            foreach (DomainEvent domainEvent in domainEvents)
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

    public class TestAggregateWithSinglePk : AggregateRoot
    {
        [EventItemPartitionKey]
        public string FirstProp { get; private set; }
        public string SecondProp { get; private set; }

        public void SetEvents(IReadOnlyList<DomainEvent> domainEvents)
        {
            foreach (DomainEvent domainEvent in domainEvents)
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

    public class TestAggregateWithMultiplePk : AggregateRoot
    {
        [EventItemPartitionKey]
        public string FirstProp { get; private set; }
        [EventItemPartitionKey]
        public string SecondProp { get; private set; }

        public void SetEvents(IReadOnlyList<Testing.SampleEvent> domainEvents)
        {
            foreach (DomainEvent domainEvent in domainEvents)
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