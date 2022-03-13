// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Aggregates;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Mappers;
using Microsoft.Azure.CosmosRepository.Extensions;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests;

public partial class AcceptanceTests
{
    private readonly string[] _names = {"List 1", "List 2", "List 3"};
    private readonly TodoListMapper _mapper = new();

    private async Task Execute()
    {
        await CreateLists();
    }

    private async Task CreateLists()
    {
        List<TodoListAggregate> todoLists = _names.Select(name =>
            new TodoListAggregate(name)).ToList();

        await _todoListItemEventStore.PersistAsync(
            todoLists[0],
            todoLists[0].Name);

        await _todoListItemEventStore.PersistAsync(
            todoLists[1],
            _mapper);

        List<TodoListEventItem> eventItems = todoLists[2]
            .NewEvents
            .Select(domainEvent =>
                new TodoListEventItem(
                    domainEvent,
                    todoLists[2].Name))
            .ToList();

        eventItems.Add(new TodoListEventItem(
            todoLists[2].AtomicEvent,
            todoLists[2].Name));

        await _todoListItemEventStore.PersistAsync(eventItems);

        List<TodoListEventItem> list1Events = await _todoListItemEventStore
            .ReadAsync(todoLists[0].Name)
            .ToListAsync();

        list1Events.Should().HaveCount(2);
        list1Events.Should().Contain(x => x.EventName == nameof(AtomicEvent));
    }
}