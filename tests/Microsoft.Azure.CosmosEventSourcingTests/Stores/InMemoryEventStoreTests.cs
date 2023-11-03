// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices.Context;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Builders;
using Microsoft.Azure.CosmosEventSourcing.ChangeFeed;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests.Stores;

public class InMemoryEventStoreTests
{
    private readonly AutoMocker _autoMocker = new();
    private readonly CosmosEventSourcingOptions _options = new();
    private readonly Mock<IContextService> _contextService;
    private readonly Mock<IChangeFeedContainerProcessorProvider> _changeFeedProcessorProvider;
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly ICosmosEventSourcingBuilder _eventSourcingBuilder;

    public InMemoryEventStoreTests()
    {
        _contextService = _autoMocker.GetMock<IContextService>();
        _changeFeedProcessorProvider = _autoMocker.GetMock<IChangeFeedContainerProcessorProvider>();
        _autoMocker.GetMock<IOptionsMonitor<CosmosEventSourcingOptions>>()
            .SetupGet(o => o.CurrentValue)
            .Returns(_options);

        _options.IsSequenceNumberingDisabled = true;

        // Required to make sure the json converter knows about all event types
        _eventSourcingBuilder = new DefaultCosmosEventSourcingBuilder(_services)
            .AddDomainEventTypes()
            .AddDomainEventProjectionHandlers();
    }

    private IEventStore<TEventItem> CreateSut<TEventItem>() where TEventItem : EventItem =>
        _autoMocker.CreateInstance<InMemoryEventStore<TEventItem>>();

    [Fact]
    public async Task PersistAsync_BrandNewEvents_AllowsThemToBeReadBack()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut<Testing.SampleEventItem>();

        _changeFeedProcessorProvider
            .Setup(o => o.GetProcessors())
            .Returns(new List<IContainerChangeFeedProcessor>());

        const string partitionKey = "a";

        List<Testing.SampleEventItem> newEvents = new()
        {
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
        };

        //Arrange
        await sut.PersistAsync(newEvents);
        IEnumerable<Testing.SampleEventItem> results = await sut.ReadAsync(partitionKey);

        //Act
        results.Should().BeEquivalentTo(newEvents);
    }

    [Fact]
    public async Task PersistAsync_AdditionalEvents_SeesTheyAreAddedAndAreReadBackCorrectly()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut<Testing.SampleEventItem>();

        _changeFeedProcessorProvider
            .Setup(o => o.GetProcessors())
            .Returns(new List<IContainerChangeFeedProcessor>());

        const string partitionKey = "a";

        List<Testing.SampleEventItem> initialEvents = new()
        {
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property 1"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property 2"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property 3"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property 4"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property 5"), partitionKey),
        };
        await sut.PersistAsync(initialEvents);

        List<Testing.SampleEventItem> newEvents = new()
        {
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property 6"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property 7"), partitionKey),
        };

        //Arrange
        await sut.PersistAsync(newEvents);
        IEnumerable<Testing.SampleEventItem> results = await sut.ReadAsync(partitionKey);

        //Act
        results.Should().HaveCount(7);
    }

    [Fact]
    public async Task PersistAsync_NewEvents_CallsChangeFeedProcessor()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut<Testing.SampleEventItem>();

        _eventSourcingBuilder
            .AddDefaultDomainEventProjection<Testing.SampleEventItem, InMemoryProjectKeyOne>();

        _services
            .AddLogging()
            .AddSingleton(new Mock<IContextService>().Object);

        var processedEvents = new List<Testing.SampleEvent>();
        _services.AddSingleton(processedEvents);

        ServiceProvider provider = _services.BuildServiceProvider();
        ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        _changeFeedProcessorProvider
            .Setup(o => o.GetProcessors())
            .Returns(new List<IContainerChangeFeedProcessor>()
            {
                new DefaultEventSourcingProcessor<Testing.SampleEventItem, InMemoryProjectKeyOne>(
                    null!,
                    null!,
                    null!,
                    loggerFactory
                        .CreateLogger<DefaultEventSourcingProcessor<Testing.SampleEventItem, InMemoryProjectKeyOne>>(),
                    provider)
            });

        const string partitionKey = "a";

        List<Testing.SampleEventItem> newEvents = new()
        {
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
        };

        //Arrange
        await sut.PersistAsync(newEvents);

        //Act
        processedEvents.Should().BeEquivalentTo(newEvents.Select(x => x.DomainEvent));
    }

    [Fact]
    public async Task PersistAsync_NewEvents_CallsMultipleChangeFeedProcessor()
    {
        //Arrange
        IEventStore<Testing.SampleEventItem> sut = CreateSut<Testing.SampleEventItem>();

        _eventSourcingBuilder
            .AddDefaultDomainEventProjection<Testing.SampleEventItem, InMemoryProjectKeyOne>()
            .AddDefaultDomainEventProjection<Testing.SampleEventItem, InMemoryProjectKeyTwo>();

        _services
            .AddLogging()
            .AddSingleton(new Mock<IContextService>().Object);

        var processedEvents = new List<Testing.SampleEvent>();
        _services.AddSingleton(processedEvents);

        ServiceProvider provider = _services.BuildServiceProvider();
        ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        _changeFeedProcessorProvider
            .Setup(o => o.GetProcessors())
            .Returns(new List<IContainerChangeFeedProcessor>()
            {
                new DefaultEventSourcingProcessor<Testing.SampleEventItem, InMemoryProjectKeyOne>(
                    null!,
                    null!,
                    null!,
                    loggerFactory
                        .CreateLogger<DefaultEventSourcingProcessor<Testing.SampleEventItem, InMemoryProjectKeyOne>>(),
                    provider),
                new DefaultEventSourcingProcessor<Testing.SampleEventItem, InMemoryProjectKeyTwo>(
                    null!,
                    null!,
                    null!,
                    loggerFactory
                        .CreateLogger<DefaultEventSourcingProcessor<Testing.SampleEventItem, InMemoryProjectKeyTwo>>(),
                    provider)
            });

        const string partitionKey = "a";

        List<Testing.SampleEventItem> newEvents = new()
        {
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
            new Testing.SampleEventItem(new Testing.SampleEvent(partitionKey, "Second Property"), partitionKey),
        };

        //Arrange
        await sut.PersistAsync(newEvents);

        //Act
        processedEvents.Should().HaveCount(10);
    }

    private class InMemoryProjectKeyOne : IProjectionKey
    {
    }

    private class InMemoryProjectionOne(List<Testing.SampleEvent> events)
        : IDomainEventProjection<Testing.SampleEvent, Testing.SampleEventItem,
            InMemoryProjectKeyOne>
    {
        public ValueTask HandleAsync(Testing.SampleEvent domainEvent, Testing.SampleEventItem eventItem,
            CancellationToken cancellationToken = default)
        {
            events.Add(domainEvent);
            return new ValueTask();
        }
    }

    private class InMemoryProjectKeyTwo : IProjectionKey
    {
    }

    private class InMemoryProjectionTwo(List<Testing.SampleEvent> events)
        : IDomainEventProjection<Testing.SampleEvent, Testing.SampleEventItem,
            InMemoryProjectKeyTwo>
    {
        public ValueTask HandleAsync(Testing.SampleEvent domainEvent, Testing.SampleEventItem eventItem,
            CancellationToken cancellationToken = default)
        {
            events.Add(domainEvent);
            return new ValueTask();
        }
    }
}