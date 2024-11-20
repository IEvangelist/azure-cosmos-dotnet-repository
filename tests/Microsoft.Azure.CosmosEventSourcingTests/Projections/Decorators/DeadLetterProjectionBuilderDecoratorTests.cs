// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Models;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests.Projections.Decorators;

public class DeadLetterProjectionBuilderDecoratorTests
{
    public class DecoratorEventItem : EventItem
    {
    }

    public class DecoratorDeadLetteredEventItem : DeadLetteredEventItem<DecoratorEventItem>
    {
        public DecoratorDeadLetteredEventItem(
            DecoratorEventItem eventItem,
            string processorName,
            string instanceName,
            string projectionKeyName,
            ExceptionDetails exception) :
            base(eventItem, processorName, instanceName, projectionKeyName, exception)
        {
        }
    }

    private record SampleDecoratorEvent : DomainEvent;


    public record DecoratorProjectionKey : IProjectionKey;

    public class DecoratorProjection : IEventItemProjection<DecoratorEventItem, DecoratorProjectionKey>
    {
        public ValueTask ProjectAsync(DecoratorEventItem eventItem, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
    }

    [Fact]
    public async Task Handle_FailedProjection_WritesToDeadLetterDb()
    {
        //Arrange
        ServiceCollection services = new();
        services.AddCosmosEventSourcing(es =>
        {
            es.AddCosmosRepository(options => options.ContainerBuilder
                    .ConfigureEventItemStore<DecoratorEventItem>("decorator-events")
                    .ConfigureDeadLetteredEventItemStore<DecoratorEventItem, DecoratorDeadLetteredEventItem>(
                        "dead-letter"));

            es.AddEventItemProjection<DecoratorEventItem, DecoratorProjectionKey, DecoratorProjection>(
                    options =>
                    {
                        options.ProcessorName = "a";
                        options.InstanceName = "b";
                    })
                .WithDeadLetterDecorator();
        });

        DomainEventConverter.ConvertibleTypes.Add(typeof(SampleDecoratorEvent));

        services.AddInMemoryCosmosRepository();

        IServiceProvider provider = services.BuildServiceProvider();

        var deadLetterRepository =
            (InMemoryRepository<DeadLetteredEventItem<DecoratorEventItem>>)
            provider.GetRequiredService<IWriteOnlyRepository<DeadLetteredEventItem<DecoratorEventItem>>>();

        IEventItemProjection<DecoratorEventItem, DecoratorProjectionKey> projection =
            provider.GetRequiredService<IEventItemProjection<DecoratorEventItem, DecoratorProjectionKey>>();

        DecoratorEventItem failedEventItem = new()
        {
            DomainEvent = new SampleDecoratorEvent
            {
                EventId = Guid.NewGuid().ToString(),
                Sequence = 1,
                OccuredUtc = DateTime.UtcNow,
            },
            PartitionKey = "A",
        };

        //Act
        await projection.ProjectAsync(failedEventItem);

        //Assert
        List<DeadLetteredEventItem<DecoratorEventItem>> results = await deadLetterRepository
            .GetAsync(x =>
                !string.IsNullOrWhiteSpace(x.Id))
            .ToListAsync();

        results[0].EventItem.Id.Should().Be(failedEventItem.Id);
    }
}