// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Events;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Projections;

public record DefaultKey : IProjectionKey;

public class TodoListEventItemProjection : IEventItemProjection<TodoListEventItem, DefaultKey>
{

    private readonly IWriteOnlyRepository<TodoListItem> _repository;
    private readonly ILogger<TodoListEventItemProjection> _logger;

    public TodoListEventItemProjection(
        IWriteOnlyRepository<TodoListItem> repository,
        ILogger<TodoListEventItemProjection> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async ValueTask ProjectAsync(
        TodoListEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        if (eventItem.DomainEvent is TodoListCreated created)
        {
            _logger.LogInformation("TodoListCreated being processed with ID {TodoId} and name {TodoName}",
                eventItem.Id,
                created.Name);

            await _repository.CreateAsync(new TodoListItem(created.Name), cancellationToken);
        }

    }
}