// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests;

public class EventSourceTests
{
    [Fact]
    public void Ctor_EmptyPartitionKey_ThrowsArgumentNullException()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new Testing.SampleEventSource(new Testing.SampleEvent(DateTime.UtcNow), string.Empty));
        ex.Message.Should().Be("The partition key must be provided (Parameter 'partitionKey')");
    }

    [Fact]
    public void Ctor_NullPartitionKey_ThrowsArgumentNullException()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new Testing.SampleEventSource(new Testing.SampleEvent(DateTime.UtcNow), null!));
        ex.Message.Should().Be("The partition key must be provided (Parameter 'partitionKey')");
    }

    [Fact]
    public void Ctor_ValidValues_CreatesSource()
    {
        //Arrange
        Testing.SampleEvent evt = new(DateTime.UtcNow);

        //Act
        Testing.SampleEventSource source = new(evt, "A");

        //Assert
        source.Id.Should().NotBeNull();
        source.PartitionKey.Should().Be("A");
        source.EventPayload.Should().Be(evt);
        source.EventName.Should().Be(evt.EventName);
    }
}