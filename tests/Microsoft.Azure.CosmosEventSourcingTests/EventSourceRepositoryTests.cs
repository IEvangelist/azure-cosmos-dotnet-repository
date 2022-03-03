// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests;

public class EventSourceRepositoryTests
{
    private readonly AutoMocker _autoMocker = new();
    private readonly Mock<IRepository<Testing.SampleEventSource>> _repository;
    private const string Pk = "pk";

    private readonly List<Testing.SampleEventSource> _events = new()
    {
        new Testing.SampleEventSource(new Testing.SampleEvent(DateTime.UtcNow), Pk),
        new Testing.SampleEventSource(new Testing.SampleEvent(DateTime.UtcNow), Pk),
        new Testing.SampleEventSource(new Testing.SampleEvent(DateTime.UtcNow), Pk),
        new Testing.SampleEventSource(new Testing.SampleEvent(DateTime.UtcNow), Pk),
        new Testing.SampleEventSource(new Testing.SampleEvent(DateTime.UtcNow), Pk),
    };

    public EventSourceRepositoryTests() =>
        _repository = _autoMocker.GetMock<IRepository<Testing.SampleEventSource>>();

    private IEventSourceRepository<Testing.SampleEventSource> CreateSut() =>
        _autoMocker.CreateInstance<EventSourceRepository<Testing.SampleEventSource>>();

    [Fact]
    public async Task PersistAsync_Events_SavesAllEvents()
    {
        //Arrange
        IEventSourceRepository<Testing.SampleEventSource> sut = CreateSut();

        //Act
        await sut.PersistAsync(_events);

        //Assert
        _repository.Verify(o => o.CreateAsync(_events, default));
    }

    [Fact]
    public async Task GetAsync_EventsInDb_GetsAllEvents()
    {
        //Arrange
        IEventSourceRepository<Testing.SampleEventSource> sut = CreateSut();

        _repository
            .Setup(o =>
                o.GetAsync(x => x.PartitionKey == Pk, default))
            .ReturnsAsync(_events);

        //Act
        IEnumerable<Testing.SampleEventSource> got = await sut.ReadAsync(Pk);

        //Assert
        got.Should().BeEquivalentTo(_events);
    }

    [Fact]
    public async Task StreamAsync_EventsInDb_StreamsAllEvents()
    {
        //Arrange
        IEventSourceRepository<Testing.SampleEventSource> sut = CreateSut();

        Page<Testing.SampleEventSource> page1 = new(
            null,
            5,
            _events,
            0,
            Guid.NewGuid().ToString());

        Page<Testing.SampleEventSource> page2 = new(
            null,
            5,
            _events,
            0,
            Guid.NewGuid().ToString());

        Page<Testing.SampleEventSource> page3 = new(
            null,
            5,
            _events.Take(2).ToList(),
            0);

        _repository
            .SetupSequence(o => o.PageAsync(
                x => x.PartitionKey == Pk,
                5,
                It.IsAny<string>(),
                false,
                default))
            .ReturnsAsync(page1)
            .ReturnsAsync(page2)
            .ReturnsAsync(page3);

        List<Testing.SampleEventSource> events = new();

        //Act
        await foreach (Testing.SampleEventSource result in sut.StreamAsync(Pk, 5))
        {
            events.Add(result);
        }

        //Assert
        events.Count.Should().Be(12);
    }
}