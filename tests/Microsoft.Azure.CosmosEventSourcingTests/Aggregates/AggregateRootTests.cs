// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests.Aggregates;

public class AggregateTests
{
    private record AggregateRootEvent(string Message) : DomainEvent;

    private record OrderedRootEvent(string Message) : DomainEvent;

    private class SampleAggregateRoot : AggregateRoot
    {
        private readonly List<string> _messages = new();

        public IReadOnlyList<string> Messages => _messages;

        public string FinalMessage { get; private set; } = null!;

        protected override void Apply(DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case AggregateRootEvent rootEvent:
                    _messages.Add(rootEvent.Message);
                    break;
                case OrderedRootEvent orderedRootEvent:
                    FinalMessage = orderedRootEvent.Message;
                    break;
            }
        }

        public static SampleAggregateRoot Replay(List<DomainEvent> domainEvents)
        {
            SampleAggregateRoot root = new();
            root.Apply(domainEvents);
            return root;
        }

        public void AddMessage(string message) =>
            AddEvent(new AggregateRootEvent(message));

        private SampleAggregateRoot()
        {

        }

        public SampleAggregateRoot(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            AddEvent(new AggregateRootEvent(message));
        }
    }

    [Fact]
    public void Ctor_NewEvent_AddsEventToNewEvents()
    {
        //Arrange
        string message = "A";

        //Act
        SampleAggregateRoot root = new(message);

        //Assert
        root.NewEvents.Should().HaveCount(1);
        root.Messages.Should().Contain("A");
    }

    [Fact]
    public void Ctor_FirstEvent_CreatesAtomicEvent()
    {
        //Arrange
        string message = "A";

        //Act
        SampleAggregateRoot root = new(message);

        //Assert
        root.AtomicEvent.Should().NotBeNull();
        root.AtomicEvent.ETag.Should().BeEmpty();
        root.AtomicEvent.Id.Should().NotBeEmpty();
        root.AtomicEvent.Sequence.Should().Be(int.MaxValue);
    }

    [Fact]
    public void Replay_ExistingEvents_AppliesCorrectly()
    {
        //Arrange
        AtomicEvent atomicEvent = new(Guid.NewGuid(), Guid.NewGuid().ToString());
        List<DomainEvent> events = new()
        {
            new AggregateRootEvent("A"),
            new AggregateRootEvent("B"),
            atomicEvent
        };

        //Act
        SampleAggregateRoot root = SampleAggregateRoot.Replay(events);

        //Assert
        root.NewEvents.Should().BeEmpty();
        root.Messages.Should().HaveCount(2);
        root.Messages.Should().Contain("A");
        root.Messages.Should().Contain("B");
        root.AtomicEvent.Should().Be(atomicEvent);
    }

    [Fact]
    public void Replay_ExistingEvents_AppliesInCorrectSequence()
    {
        //Arrange
        AtomicEvent atomicEvent = new(Guid.NewGuid(), Guid.NewGuid().ToString());
        List<DomainEvent> events = new()
        {
            new OrderedRootEvent("A") {Sequence = 1},
            new OrderedRootEvent("B") {Sequence = 2},
            new OrderedRootEvent("C") {Sequence = 3},
            new OrderedRootEvent("D") {Sequence = 4},
            atomicEvent
        };

        //Act
        SampleAggregateRoot root = SampleAggregateRoot.Replay(events);

        //Assert
        root.FinalMessage.Should().Be("D");
    }

    [Fact]
    public void Replay_ExistingEventsWithNoAtomicEvent_ThrowsAtomicEventRequiredException()
    {
        //Arrange
        List<DomainEvent> events = new()
        {
            new OrderedRootEvent("A") {Sequence = 1},
            new OrderedRootEvent("B") {Sequence = 2},
            new OrderedRootEvent("C") {Sequence = 3},
            new OrderedRootEvent("D") {Sequence = 4},
        };

        //Act
        //Assert
        Assert.Throws<AtomicEventRequiredException>(() =>
            SampleAggregateRoot.Replay(events));
    }

    [Fact]
    public void Replay_NoEvents_ThrowsDomainEventsRequiredException()
    {
        //Arrange
        List<DomainEvent> events = new();

        //Act
        //Assert
        Assert.Throws<DomainEventsRequiredException>(() =>
            SampleAggregateRoot.Replay(events));
    }

    [Fact]
    public void AddNewEvent_AggregateRootHasBeenReplayedExistingEvents_AddsNewEventAndSetsAtomicEventProperty()
    {
        //Arrange
        AtomicEvent atomicEvent = new(Guid.NewGuid(), Guid.NewGuid().ToString());
        List<DomainEvent> events = new()
        {
            new OrderedRootEvent("A") {Sequence = 1},
            new OrderedRootEvent("B") {Sequence = 2},
            new OrderedRootEvent("C") {Sequence = 3},
            new OrderedRootEvent("D") {Sequence = 4},
            atomicEvent
        };

        SampleAggregateRoot root = SampleAggregateRoot.Replay(events);

        //Act
        root.AddMessage("E");


        //Assert
        root.Messages.Should().Contain("E");
        root.AtomicEvent.Id.Should().Be(atomicEvent.Id);
        root.AtomicEvent.OccuredUtc.Should().NotBe(atomicEvent.OccuredUtc);
    }

}