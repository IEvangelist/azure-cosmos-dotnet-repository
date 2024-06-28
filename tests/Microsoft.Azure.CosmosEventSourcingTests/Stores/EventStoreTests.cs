// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices.Context;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests.Stores;

public partial class EventStoreTests
{
    private readonly AutoMocker _autoMocker = new();
    private readonly Mock<IBatchRepository<Testing.SampleEventItem>> _batchRepository;
    private readonly Mock<IReadOnlyRepository<Testing.SampleEventItem>> _readonlyRepository;
    private readonly Mock<IContextService> _contextService;
    private const string Pk = "pk";

    private readonly List<Testing.SampleEvent> _events =
    [
        new Testing.SampleEvent(Pk, "Second Property"),
        new Testing.SampleEvent(Pk, "Second Property"),
        new Testing.SampleEvent(Pk, "Second Property"),
        new Testing.SampleEvent(Pk, "Second Property"),
        new Testing.SampleEvent(Pk, "Second Property"),
    ];

    private readonly List<Testing.SampleEventItem> _eventItems =
    [
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
    ];

    private readonly List<Testing.SampleEventItem> _eventItemsWithAtomicEvents =
    [
        new Testing.SampleEventItem(new AtomicEvent(Guid.NewGuid().ToString(), string.Empty), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
        new Testing.SampleEventItem(new Testing.SampleEvent(Pk, "Second Property"), Pk),
    ];

    public EventStoreTests()
    {
        _batchRepository = _autoMocker.GetMock<IBatchRepository<Testing.SampleEventItem>>();
        _readonlyRepository = _autoMocker.GetMock<IReadOnlyRepository<Testing.SampleEventItem>>();
        _contextService = _autoMocker.GetMock<IContextService>();
    }

    private IEventStore<Testing.SampleEventItem> CreateSut() =>
        _autoMocker.CreateInstance<DefaultEventStore<Testing.SampleEventItem>>();

    [Fact]
    public async Task PersistAsync_Events_SavesAllEvents()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        //Act
        await sut.PersistAsync(_eventItemsWithAtomicEvents);

        //Assert
        _batchRepository.Verify(o =>
            o.UpdateAsBatchAsync(
                _eventItemsWithAtomicEvents,
                default));
    }

    [Fact]
    public async Task PersistAsync_EventsAndCorrelationId_SavesAllEventsWithCorrelationId()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        var correlationId = Guid.NewGuid().ToString();
        _contextService.SetupGet(o => o.CorrelationId).Returns(correlationId);

        //Act
        await sut.PersistAsync(_eventItemsWithAtomicEvents);

        //Assert
        _batchRepository.Verify(o =>
            o.UpdateAsBatchAsync(
                _eventItemsWithAtomicEvents,
                default));

        _eventItemsWithAtomicEvents
            .Where(x => x.DomainEvent is not AtomicEvent)
            .All(x => x.CorrelationId == correlationId)
            .Should().BeTrue();
    }

    [Fact]
    public async Task PersistAsync_NoEvents_DoesNotSaveBatch()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        //Act
        await sut.PersistAsync(new List<Testing.SampleEventItem>());

        //Assert
        _batchRepository.Verify(o =>
            o.UpdateAsBatchAsync(
                It.IsAny<List<Testing.SampleEventItem>>(),
                default),
            Times.Never);
    }

    [Fact]
    public async Task PersistAsync_EventsWithAtomicEvent_ThrowAtomicEventRequiredExceptions()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        //Act
        //Assert
        await Assert.ThrowsAsync<AtomicEventRequiredException>(() =>
            sut.PersistAsync(_eventItems).AsTask());
    }

    [Fact]
    public async Task GetAsync_EventsInDb_GetsAllEvents()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        _readonlyRepository
            .Setup(o =>
                o.GetAsync(x => x.PartitionKey == Pk, default))
            .ReturnsAsync(_eventItemsWithAtomicEvents);

        //Act
        IEnumerable<Testing.SampleEventItem> got = await sut.ReadAsync(Pk);

        //Assert
        got.Should().BeEquivalentTo(_eventItemsWithAtomicEvents);
    }

    [Fact]
    public async Task StreamAsync_EventsInDb_StreamsAllEvents()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        Page<Testing.SampleEventItem> page1 = new(
            null,
            5,
            _eventItemsWithAtomicEvents,
            0,
            Guid.NewGuid().ToString());

        Page<Testing.SampleEventItem> page2 = new(
            null,
            5,
            _eventItems,
            0,
            Guid.NewGuid().ToString());

        Page<Testing.SampleEventItem> page3 = new(
            null,
            5,
            _eventItems.Take(2).ToList(),
            0);

        _readonlyRepository
            .SetupSequence(o => o.PageAsync(
                x => x.PartitionKey == Pk,
                5,
                It.IsAny<string>(),
                false,
                default))
            .ReturnsAsync(page1)
            .ReturnsAsync(page2)
            .ReturnsAsync(page3);

        List<Testing.SampleEventItem> events = [];

        //Act
        await foreach (Testing.SampleEventItem result in sut.StreamAsync(Pk, 5))
        {
            events.Add(result);
        }

        //Assert
        events.Count.Should().Be(13);
    }
}