// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests.Items;

public class EventItemTests
{
    [Fact]
    public void Ctor_EmptyPartitionKey_ThrowsArgumentNullException()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new Testing.SampleEventItem(new Testing.SampleEvent(), string.Empty));
        ex.Message.Should().Be("The partition key must be provided (Parameter 'partitionKey')");
    }

    [Fact]
    public void Ctor_NullPartitionKey_ThrowsArgumentNullException()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new Testing.SampleEventItem(new Testing.SampleEvent(), null!));
        ex.Message.Should().Be("The partition key must be provided (Parameter 'partitionKey')");
    }

    [Fact]
    public void Ctor_ValidValues_CreatesSource()
    {
        //Arrange
        Testing.SampleEvent evt = new();

        //Act
        Testing.SampleEventItem item = new(evt, "A");

        //Assert
        item.Id.Should().NotBeNull();
        item.PartitionKey.Should().Be("A");
        item.EventPayload.Should().Be(evt);
        item.EventName.Should().Be(evt.EventName);
    }
}