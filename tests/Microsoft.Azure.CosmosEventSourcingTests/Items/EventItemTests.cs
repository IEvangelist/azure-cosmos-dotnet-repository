// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xunit;
using static Microsoft.Azure.CosmosEventSourcingTests.Testing;

namespace Microsoft.Azure.CosmosEventSourcingTests.Items;

public class EventItemTests
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    [Fact]
    public void Ctor_ValidValues_CreatesSource()
    {
        //Arrange
        SampleEvent evt = new();

        //Act
        SampleEventItem item = new(evt, "A");

        //Assert
        item.Id.Should().NotBeNull();
        item.PartitionKey.Should().Be("A");
        item.DomainEvent.Should().Be(evt);
        item.EventName.Should().Be(evt.EventName);
    }

    [Fact]
    public void EventItem_Serialization_WorksCorrectly()
    {
        //Arrange
        DomainEventConverter.ConvertableTypes.Add(typeof(SampleEvent));
        SampleEvent evt = new()
        {
            EventId = Guid.NewGuid().ToString(),
            Sequence = 1,
            OccuredUtc = DateTime.UtcNow
        };

        SampleEventItem item = new(evt, "A");

        //Act
        string json = JsonConvert.SerializeObject(item, _jsonSerializerSettings);

        //Assert
        SampleEventItem deserialized = JsonConvert.DeserializeObject<SampleEventItem>(json, _jsonSerializerSettings);
        deserialized.DomainEvent.EventId.Should().Be(evt.EventId);
        deserialized.Id.Should().Be(evt.EventId);
        deserialized.DomainEvent.Sequence.Should().Be(1);
        deserialized.DomainEvent.OccuredUtc.Should().Be(evt.OccuredUtc);
    }

    [Fact]
    public void EventItem_AtomicEventSerialization_WorksCorrectly()
    {
        //Arrange
        DomainEventConverter.ConvertableTypes.Add(typeof(AtomicEvent));
        string etagValue = Guid.NewGuid().ToString();
        AtomicEvent evt = new(Guid.NewGuid().ToString(), string.Empty)
        {
            Sequence = int.MaxValue,
            OccuredUtc = DateTime.UtcNow,
            ETag = etagValue
        };

        SampleEventItem item = new(evt, "A");

        //Act
        string json = JsonConvert.SerializeObject(item, _jsonSerializerSettings);

        //Assert
        SampleEventItem deserialized = JsonConvert.DeserializeObject<SampleEventItem>(json, _jsonSerializerSettings);
        deserialized.DomainEvent.EventId.Should().Be(evt.EventId);
        deserialized.Id.Should().Be(evt.EventId);
        deserialized.DomainEvent.Sequence.Should().Be(int.MaxValue);
        deserialized.DomainEvent.OccuredUtc.Should().Be(evt.OccuredUtc);
        deserialized.Etag.Should().Be(etagValue);
    }

    [Fact]
    public void EventItem_AtomicEventDeserialization_WorksCorrectly()
    {
        //Arrange
        DomainEventConverter.ConvertableTypes.Add(typeof(AtomicEvent));
        string etagValue = Guid.NewGuid().ToString();
        AtomicEvent atomicEvent = new(nameof(AtomicEvent), null!)
        {
            Sequence = int.MaxValue,
            OccuredUtc = DateTime.UtcNow
        };

        string? json = JObject.FromObject(new
        {
            _etag = etagValue,
            domainEvent = atomicEvent,
            id = "123",
            eventName = nameof(AtomicEvent)
        }).ToString();

        //Act
        SampleEventItem? item = JsonConvert.DeserializeObject<SampleEventItem>(json);

        //Assert
        item.Etag.Should().Be(etagValue);
        AtomicEvent ae = (AtomicEvent) item.DomainEvent;
        ae.ETag.Should().Be(etagValue);
    }

    [Fact]
    public void EventItem_AtomicEventPass_WorksCorrectly()
    {
        //Arrange
        DomainEventConverter.ConvertableTypes.Add(typeof(AtomicEvent));
        string etagValue = Guid.NewGuid().ToString();
        AtomicEvent atomicEvent = new(nameof(AtomicEvent), null!)
        {
            Sequence = int.MaxValue,
            OccuredUtc = DateTime.UtcNow
        };

        string? json = JObject.FromObject(new
        {
            _etag = etagValue,
            domainEvent = atomicEvent,
            id = "123",
            eventName = nameof(AtomicEvent)
        }).ToString();

        //Act
        SampleEventItem? item = JsonConvert.DeserializeObject<SampleEventItem>(json);
        AtomicEvent ae = (AtomicEvent) item.DomainEvent;
        SampleEventItem newEventItem = new(ae, "123");
        string? reSerialized = JsonConvert.SerializeObject(newEventItem);

        //Assert
        JObject eventItemJson = JObject.Parse(reSerialized);
        eventItemJson["_etag"].Value<string>().Should().Be(etagValue);
    }
}