// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Attributes;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests.Stores;

public partial class EventStoreTests
{
    [Fact]
    public async Task PersistAsync_AggregateWithAttribute_SavesAllEvents()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        Testing.TestAggregateWithSinglePk aggregate = new();
        aggregate.SetEvents(_events);

        //Act
        await sut.PersistAsync(aggregate);

        //Assert
        _batchRepository.Verify(o =>
            o.UpdateAsBatchAsync(
                It.Is<IEnumerable<Testing.SampleEventItem>>(x =>

                    x.All(y => y.PartitionKey == aggregate.FirstProp)),
                default),
            Times.Once);
    }

    [Fact]
    public async Task PersistAsync_AggregateWithAttributeAndNullPk_ThrowsInvalidPartitionKeyValueException()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        Testing.TestAggregateWithSinglePk aggregate = new();
        aggregate.SetEvents(_events);
        aggregate.AddEvent(new Testing.SampleEvent(null!));

        //Act & Assert
        InvalidPartitionKeyValueException exception =
            await Assert.ThrowsAsync<InvalidPartitionKeyValueException>(async () =>
                await sut.PersistAsync(aggregate));

        exception.Message
            .Should()
            .Be($"{nameof(Testing.TestAggregateWithSinglePk.FirstProp)} in {nameof(Testing.TestAggregateWithSinglePk)} was null. This is not a valid partition key value");
    }

    [Fact]
    public async Task PersistAsync_Aggregate_SavesAllEvents()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        Testing.TestAggregateWithNoPk aggregate = new();
        aggregate.SetEvents(_events);

        //Act
        await sut.PersistAsync(aggregate, aggregate.FirstProp);

        //Assert
        _batchRepository.Verify(o =>
                o.UpdateAsBatchAsync(
                    It.Is<IEnumerable<Testing.SampleEventItem>>(x =>
                        x.All(y => y.PartitionKey == aggregate.FirstProp)),
                    default),
            Times.Once);
    }

    [Fact]
    public async Task PersistAsync_AggregateWhenCorrelationIdIsSet_SavesAllEventsWithCorrelationId()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        var correlationId = Guid.NewGuid().ToString();
        _contextService.SetupGet(o => o.CorrelationId).Returns(correlationId);

        Testing.TestAggregateWithNoPk aggregate = new();
        aggregate.SetEvents(_events);

        //Act
        await sut.PersistAsync(aggregate, aggregate.FirstProp);

        //Assert
        _batchRepository.Verify(o =>
                o.UpdateAsBatchAsync(
                    It.Is<IEnumerable<Testing.SampleEventItem>>(x =>
                        x.Where(e => !(e.DomainEvent is AtomicEvent))
                        .All(e => e.CorrelationId == correlationId)),
                    default),
            Times.Once);

    }

    [Fact]
    public async Task PersistAsync_AggregateWithTwoAttributes_ThrowsInvalidEventItemPartitionKeyAttributeCombinationException()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        Testing.TestAggregateWithMultiplePk aggregate = new();
        aggregate.SetEvents(_events);

        //Act & Assert
        InvalidEventItemPartitionKeyAttributeCombinationException exception =
            await Assert.ThrowsAsync<InvalidEventItemPartitionKeyAttributeCombinationException>(async () =>
                await sut.PersistAsync(aggregate));

        exception.Message
            .Should()
            .Be(
                $"{nameof(EventItemPartitionKeyAttribute)} can not be present on multiple properties in {nameof(Testing.TestAggregateWithMultiplePk)}");
    }

    [Fact]
    public async Task PersistAsync_AggregateWithNoAttributes_ThrowsEventItemPartitionKeyAttributeRequiredException()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        Testing.TestAggregateWithNoPk aggregate = new();
        aggregate.SetEvents(_events);

        //Act & Assert
        EventItemPartitionKeyAttributeRequiredException exception =
            await Assert.ThrowsAsync<EventItemPartitionKeyAttributeRequiredException>(async () =>
                await sut.PersistAsync(aggregate));

        exception.Message
            .Should()
            .Be(
                $"A {nameof(EventItemPartitionKeyAttribute)} must be present on a property in {nameof(Testing.TestAggregateWithNoPk)} or you must specify the partition key explicitly");
    }
}