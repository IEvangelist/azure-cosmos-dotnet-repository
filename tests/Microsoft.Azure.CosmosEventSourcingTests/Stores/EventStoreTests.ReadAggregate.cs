// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosEventSourcingTests.Extensions;
using Microsoft.Azure.CosmosRepository;
using Moq;
using Xunit;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Microsoft.Azure.CosmosEventSourcingTests.Stores;

public partial class EventStoreTests
{
    private record ReplayableEvent : DomainEvent;

    internal class ReplayableEventItem : EventItem
    {
        public ReplayableEventItem(
            DomainEvent domainEvent,
            string partitionKey)
        {
            PartitionKey = partitionKey;
            DomainEvent = domainEvent;
        }
    }


    private class TestAggregate : AggregateRoot
    {
        public int ReplayedEvents { get; private set; }

        protected override void Apply(DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case ReplayableEvent:
                    ReplayedEvents++;
                    break;
            }
        }

        public static TestAggregate Replay(List<DomainEvent> domainEvents)
        {
            TestAggregate a = new();
            a.Apply(domainEvents);
            return a;
        }

        private TestAggregate()
        {
        }
    }

    private class TestAggregateNoSequencing : AggregateRoot
    {
        public int ReplayedEvents { get; private set; }

        protected override void Apply(DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case ReplayableEvent:
                    ReplayedEvents++;
                    break;
            }
        }

        public static TestAggregateNoSequencing Replay(List<DomainEvent> domainEvents)
        {
            TestAggregateNoSequencing a = new();
            a.Apply(domainEvents);
            return a;
        }

        private TestAggregateNoSequencing() : base(isSequenceNumberDisabled: true)
        {
        }
    }

    private class TestAggregateRootMapper : IAggregateRootMapper<TestAggregate, ReplayableEventItem>
    {
        public IEnumerable<ReplayableEventItem> MapFrom(TestAggregate aggregateRoot) => throw new NotImplementedException();

        public TestAggregate MapTo(IEnumerable<ReplayableEventItem> domainEvents) =>
            TestAggregate.Replay(domainEvents.Select(x => x.DomainEvent).ToList());
    }

    private class TestAggregateRootNoSequencingMapper : IAggregateRootMapper<TestAggregateNoSequencing, ReplayableEventItem>
    {
        public IEnumerable<ReplayableEventItem> MapFrom(
            TestAggregateNoSequencing aggregateRoot) => throw new NotImplementedException();

        public TestAggregateNoSequencing MapTo(
            IEnumerable<ReplayableEventItem> domainEvents) =>
            TestAggregateNoSequencing.Replay(domainEvents.Select(x => x.DomainEvent).ToList());
    }

    private class AggregateWithNoReplayMethod : AggregateRoot
    {
        protected override void Apply(DomainEvent domainEvent) => throw new NotImplementedException();
    }

    [Fact]
    public async Task ReadAggregateAsync_AggregateWithNoReplayMethod_ThrowsReplayMethodNotDefinedException()
    {
        //Arrange
        IEventStore<ReplayableEventItem> sut = _autoMocker.CreateInstance<DefaultEventStore<ReplayableEventItem>>();

        //Act
        //Assert
        await Assert.ThrowsAsync<ReplayMethodNotDefinedException>(() =>
            sut.ReadAggregateAsync<AggregateWithNoReplayMethod>("A").AsTask());
    }
}