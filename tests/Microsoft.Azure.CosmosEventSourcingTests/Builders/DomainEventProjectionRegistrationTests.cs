// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.Azure.CosmosEventSourcingTests.Builders;

internal sealed record InternalProjectionEvent : DomainEvent;

internal sealed class InternalProjectionEventItem : EventItem
{
}

internal sealed record InternalProjectionKey : IProjectionKey;

internal sealed class InternalDomainEventProjection :
    IDomainEventProjection<InternalProjectionEvent, InternalProjectionEventItem, InternalProjectionKey>
{
    public ValueTask HandleAsync(
        InternalProjectionEvent domainEvent,
        InternalProjectionEventItem eventItem,
        CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
}

public class DomainEventProjectionRegistrationTests
{
    [Fact]
    public void AddDomainEventProjectionHandlers_RegistersInternalProjections()
    {
        ServiceCollection services = new();
        services.AddCosmosEventSourcing(builder =>
            builder.AddDomainEventProjectionHandlers(Assembly.GetExecutingAssembly()));

        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.IsType<InternalDomainEventProjection>(
            provider.GetRequiredService<IDomainEventProjection<
                InternalProjectionEvent,
                InternalProjectionEventItem,
                InternalProjectionKey>>());
    }
}
