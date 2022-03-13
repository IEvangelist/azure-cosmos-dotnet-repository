// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
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
    private readonly List<Guid> _atomicEventIds = new();

    private async Task Execute()
    {
        await CreateAndVerifyTodoItemLists();
        await AddItemToAllListsAndVerify("Task 1");
    }

    private async Task AddItemToAllListsAndVerify(string title)
    {
        foreach (string name in _names)
        {
            TodoListAggregate list = await _todoListItemEventStore.ReadAggregateAsync(name, _mapper);

            list.AddItem(title);

            await _todoListItemEventStore.PersistAsync(list, list.Name);

            list = await _todoListItemEventStore.ReadAggregateAsync<TodoListAggregate>(name);
            list.Items.Should().Contain(x => x.Title == title);
            list.Items.Count.Should().Be(1);
            list.Events.Should().HaveCount(2);
            _atomicEventIds.Should().Contain(list.AtomicEvent.Id);
        }
    }

    private async Task CreateAndVerifyTodoItemLists()
    {
        List<TodoListAggregate> todoLists = _names.Select(name =>
            new TodoListAggregate(name)).ToList();

        await _todoListItemEventStore.PersistAsync(
            todoLists[0],
            todoLists[0].Name);

        await _todoListItemEventStore.PersistAsync(
            todoLists[1],
            _mapper);

        await _todoListItemEventStore.PersistAsync(_mapper.MapFrom(todoLists[2]));

        List<TodoListEventItem> list1Events = await _todoListItemEventStore
            .ReadAsync(todoLists[0].Name)
            .ToListAsync();

        list1Events.Should().HaveCount(2);
        list1Events.Should().Contain(x => x.EventName == nameof(AtomicEvent));
        list1Events.Should().AllSatisfy(x => x.PartitionKey.Should().Be(_names[0]));
        list1Events.Should().AllSatisfy(x => x.Type.Should().Be(nameof(TodoListEventItem)));

        TodoListAggregate list1 = _mapper.MapTo(list1Events);
        list1.Name.Should().Be(_names[0]);
        list1.Items.Should().BeEmpty();
        list1.NewEvents.Should().BeEmpty();
        list1.Events.Should().HaveCount(1);
        _atomicEventIds.Add(list1.AtomicEvent.Id);

        TodoListAggregate list2 = await _todoListItemEventStore.ReadAggregateAsync(_names[1], _mapper);
        list2.Name.Should().Be(_names[1]);
        list2.Items.Should().BeEmpty();
        list2.NewEvents.Should().BeEmpty();
        list2.Events.Should().HaveCount(1);
        _atomicEventIds.Add(list2.AtomicEvent.Id);

        TodoListAggregate list3 = await _todoListItemEventStore.ReadAggregateAsync<TodoListAggregate>(_names[2]);
        list3.Name.Should().Be(_names[2]);
        list3.Items.Should().BeEmpty();
        list3.NewEvents.Should().BeEmpty();
        list3.Events.Should().HaveCount(1);
        _atomicEventIds.Add(list3.AtomicEvent.Id);
    }
}