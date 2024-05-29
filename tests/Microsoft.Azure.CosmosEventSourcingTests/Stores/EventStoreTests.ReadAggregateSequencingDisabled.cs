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
    [Fact]
    public async Task ReadAggregateAsync_AggregateWithReplayMethodNoAtomicEvent_ReplaysEvents()
    {
        //Arrange
        IEventStore<ReplayableEventItem> sut = _autoMocker.CreateInstance<DefaultEventStore<ReplayableEventItem>>();

        Mock<IReadOnlyRepository<ReplayableEventItem>> repository = _autoMocker.GetMock<IReadOnlyRepository<ReplayableEventItem>>();

        List<ReplayableEventItem> events = new()
        {
            new ReplayableEventItem(new ReplayableEvent(), "A"),
        };

        repository
            .Setup(o =>
                o.GetAsync(x => x.PartitionKey == "A", default))
            .ReturnsAsync(events);

        //Act
        TestAggregateNoSequencing a = await sut.ReadAggregateAsync<TestAggregateNoSequencing>("A");

        //Assert
        a.ReplayedEvents.Should().Be(1);
    }

    [Fact]
    public async Task ReadAggregateAsync_AggregateMapperNoAtomicEvent_MapsAggregateCorrectly()
    {
        //Arrange
        IEventStore<ReplayableEventItem> sut = _autoMocker.CreateInstance<DefaultEventStore<ReplayableEventItem>>();

        Mock<IReadOnlyRepository<ReplayableEventItem>> repository = _autoMocker.GetMock<IReadOnlyRepository<ReplayableEventItem>>();

        List<ReplayableEventItem> events = new()
        {
            new ReplayableEventItem(new ReplayableEvent(), "A"),
        };

        repository
            .Setup(o =>
                o.GetAsync(x => x.PartitionKey == "A", default))
            .ReturnsAsync(events);

        //Act
        TestAggregateNoSequencing a = await sut.ReadAggregateAsync("A", new TestAggregateRootNoSequencingMapper());

        //Assert
        a.ReplayedEvents.Should().Be(1);
    }
}