// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Aggregates;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Mappers;

public class TodoListMapper : IAggregateRootMapper<TodoListAggregate, TodoListEventItem>
{
    public IEnumerable<TodoListEventItem> MapFrom(TodoListAggregate aggregateRoot)
    {
        var eventItems = aggregateRoot
            .NewEvents
            .Select(domainEvent =>
                new TodoListEventItem(
                    domainEvent,
                    aggregateRoot.Name))
            .ToList();

        eventItems.Add(new TodoListEventItem(
            aggregateRoot.AtomicEvent,
            aggregateRoot.Name));

        return eventItems;
    }

    public TodoListAggregate MapTo(IEnumerable<TodoListEventItem> events) =>
        TodoListAggregate.Replay(events.Select(x => x.DomainEvent).ToList());
}