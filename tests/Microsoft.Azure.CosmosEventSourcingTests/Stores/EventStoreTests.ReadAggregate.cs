// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosRepository;
using Moq;
using Xunit;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Microsoft.Azure.CosmosEventSourcingTests.Stores;

public partial class EventStoreTests
{
    private record ReplayableEvent : DomainEvent;

    internal class ReplayableEventItem : DefaultEventItem
    {
        public ReplayableEventItem(
            IDomainEvent eventPayload,
            string partitionKey) :
            base(eventPayload, partitionKey)
        {
        }
    }


    private class ReplayAggregate : AggregateRoot
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

        public static ReplayAggregate Replay(List<DomainEvent> domainEvents)
        {
            ReplayAggregate a = new();
            a.Apply(domainEvents);
            return a;
        }

        private ReplayAggregate()
        {
        }
    }

    private class AggregateWithNoReplayMethod : AggregateRoot
    {
        protected override void Apply(DomainEvent domainEvent)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public async Task ReadAggregateAsync_AggregateWithReplayMethod_ReplaysEvents()
    {
        //Arrange
        IEventStore<ReplayableEventItem> sut = _autoMocker.CreateInstance<DefaultEventStore<ReplayableEventItem>>();

        Mock<IRepository<ReplayableEventItem>> repository = _autoMocker.GetMock<IRepository<ReplayableEventItem>>();

        List<ReplayableEventItem> events = new()
        {
            new(new ReplayableEvent(), "A"),
            new(new AtomicEvent(Guid.Empty, ""), "A"),
        };

        repository
            .Setup(o =>
                o.GetAsync(x => x.PartitionKey == "A", default))
            .ReturnsAsync(events);

        //Act
        ReplayAggregate a = await sut.ReadAggregateAsync<ReplayAggregate>("A");

        //Assert
        a.ReplayedEvents.Should().Be(1);
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