// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Events;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Projections;

public record CompletedProjectionsKey : IProjectionKey;

public class CompletedProjections : IDomainEventProjection<TodoItemCompleted, TodoListEventItem, CompletedProjectionsKey>
{
    private readonly ILogger<CompletedProjections> _logger;
    public static int Invocations { get; set; }

    public CompletedProjections(ILogger<CompletedProjections> logger)
    {
        _logger = logger;
    }

    public ValueTask HandleAsync(
        TodoItemCompleted domainEvent,
        TodoListEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completed projection processed");
        Invocations++;
        return ValueTask.CompletedTask;
    }
}