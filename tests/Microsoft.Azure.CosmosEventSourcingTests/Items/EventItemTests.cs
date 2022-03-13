// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests.Items;

public class EventItemTests
{
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
        item.DomainEvent.Should().Be(evt);
        item.EventName.Should().Be(evt.EventName);
    }
}