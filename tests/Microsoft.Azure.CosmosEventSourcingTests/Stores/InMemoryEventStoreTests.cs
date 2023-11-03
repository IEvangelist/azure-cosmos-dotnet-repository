// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.CompilerServices.Context;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
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

    public InMemoryEventStoreTests()
    {
        _contextService = _autoMocker.GetMock<IContextService>();
        _changeFeedProcessorProvider = _autoMocker.GetMock<IChangeFeedContainerProcessorProvider>();
        _autoMocker.GetMock<IOptionsMonitor<CosmosEventSourcingOptions>>()
            .SetupGet(o => o.CurrentValue)
            .Returns(_options);
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
}