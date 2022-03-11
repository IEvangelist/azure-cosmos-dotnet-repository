// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Attributes;
using Microsoft.Azure.CosmosEventSourcing.Events;
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
        _repository.Verify(o =>
            o.UpdateAsBatchAsync(
                It.Is<IEnumerable<Testing.SampleEventItem>>(x =>

                    x.All(y => y.PartitionKey == aggregate.FirstProp)),
                default),
            Times.Once);
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
        _repository.Verify(o =>
                o.UpdateAsBatchAsync(
                    It.Is<IEnumerable<Testing.SampleEventItem>>(x =>
                        x.All(y => y.PartitionKey == aggregate.FirstProp)),
                    default),
            Times.Once);
    }

    [Fact]
    public async Task PersistAsync_AggregateWithTwoAttributes_ThrowsInvalidOperationException()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        Testing.TestAggregateWithMultiplePk aggregate = new();
        aggregate.SetEvents(_events);

        //Act & Assert
        InvalidOperationException exception =
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.PersistAsync(aggregate));
        exception.Message
            .Should()
            .Be(
                $"{nameof(EventItemPartitionKeyAttribute)} can not be present on multiple properties in {nameof(Testing.TestAggregateWithMultiplePk)}");
    }

    [Fact]
    public async Task PersistAsync_AggregateWithNoAttributes_ThrowsInvalidOperationException()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut();

        Testing.TestAggregateWithNoPk aggregate = new();
        aggregate.SetEvents(_events);

        //Act & Assert
        InvalidOperationException exception =
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.PersistAsync(aggregate));
        exception.Message
            .Should()
            .Be(
                $"A {nameof(EventItemPartitionKeyAttribute)} must be present on a property in {nameof(Testing.TestAggregateWithNoPk)}");
    }

    private bool FluentAssertionAsBool(Action action)
    {
        try
        {
            action.Invoke();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}