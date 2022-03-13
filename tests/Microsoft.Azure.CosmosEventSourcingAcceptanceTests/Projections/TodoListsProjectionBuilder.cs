// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Events;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;
using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Projections;

public class TodoListsProjectionBuilder : IEventItemProjectionBuilder<TodoListEventItem>
{
    private readonly IWriteOnlyRepository<TodoListItem> _repository;

    public TodoListsProjectionBuilder(IWriteOnlyRepository<TodoListItem> repository) =>
        _repository = repository;

    public async ValueTask ProjectAsync(
        TodoListEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        if (eventItem.DomainEvent is TodoListCreated created)
        {
            await _repository.CreateAsync(new TodoListItem(created.Name), cancellationToken);
        }
    }
}