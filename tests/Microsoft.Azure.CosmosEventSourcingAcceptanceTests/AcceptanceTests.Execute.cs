// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Aggregates;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Mappers;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Projections;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.Logging;
using Polly;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests;

public partial class AcceptanceTests
{
    private readonly string[] _names = ["List 1", "List 2", "List 3"];
    private readonly TodoListMapper _mapper = new();
    private readonly List<string> _atomicEventIds = [];

    private readonly AsyncPolicy _defaultPolicy = Policy
        .Handle<CosmosException>()
        .WaitAndRetryAsync(20, i => TimeSpan.FromSeconds(i * 2));

    private async Task Execute()
    {
        await CreateAndVerifyTodoItemLists();
        await AddItemToAllListsAndVerify("Task 1");
        await CompleteTaskForItems(1);
        await _defaultPolicy.ExecuteAsync(CheckTodoListsProjectionBuilder);

        await Task.Delay(TimeSpan.FromSeconds(5));

        await _defaultPolicy.ExecuteAsync(() => CheckTodoItemsProjectionBuilders("Task 1"));
    }

    private async Task CreateAndVerifyTodoItemLists()
    {
        var todoLists = _names.Select(name =>
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
        _atomicEventIds.Add(list1.AtomicEvent.EventId);

        TodoListAggregate list2 = await _todoListItemEventStore.ReadAggregateAsync(_names[1], _mapper);
        list2.Name.Should().Be(_names[1]);
        list2.Items.Should().BeEmpty();
        list2.NewEvents.Should().BeEmpty();
        list2.Events.Should().HaveCount(1);
        _atomicEventIds.Add(list2.AtomicEvent.EventId);

        TodoListAggregate list3 = await _todoListItemEventStore.ReadAggregateAsync<TodoListAggregate>(_names[2]);
        list3.Name.Should().Be(_names[2]);
        list3.Items.Should().BeEmpty();
        list3.NewEvents.Should().BeEmpty();
        list3.Events.Should().HaveCount(1);
        _atomicEventIds.Add(list3.AtomicEvent.EventId);
    }

    private async Task AddItemToAllListsAndVerify(string title)
    {
        foreach (var name in _names)
        {
            TodoListAggregate list = await _todoListItemEventStore.ReadAggregateAsync(name, _mapper);

            list.AddItem(title);

            await _todoListItemEventStore.PersistAsync(list, list.Name);

            list = await _todoListItemEventStore.ReadAggregateAsync<TodoListAggregate>(name);
            list.Items.Should().Contain(x => x.Title == title);
            list.Items.Count.Should().Be(1);
            list.Events.Should().HaveCount(2);
            _atomicEventIds.Should().Contain(list.AtomicEvent.EventId);
        }
    }

    private async Task CompleteTaskForItems(int taskId)
    {
        foreach (var name in _names)
        {
            TodoListAggregate list = await _todoListItemEventStore.ReadAggregateAsync(name, _mapper);

            list.CompleteItem(taskId);

            await _todoListItemEventStore.PersistAsync(list, list.Name);
        }
    }

    private async Task CheckTodoListsProjectionBuilder()
    {
        _logger.LogInformation("Checking todo list projections");
        foreach (var name in _names)
        {
            TodoListItem list = await _todoListItemRepository.GetAsync(name, nameof(TodoListItem));
            list.Name.Should().Be(name);
        }
    }

    private async Task CheckTodoItemsProjectionBuilders(string taskTitle)
    {
        _logger.LogInformation("Checking todo items (complete/created) projections");

        foreach (var name in _names)
        {
            IEnumerable<TodoCosmosItem> items =
                await _todoItemsRepository.GetAsync(x => x.PartitionKey == name);

            items.Should().Contain(x => x.Title == taskTitle);
            items.Should().Contain(x => x.IsComplete);
            items.Should().HaveCount(1);
        }

        // It's ok to have more invocations, but we need at least 3.
        // We can end up with more when Polly retries.
        CompletedProjections.Invocations.Should().BeGreaterThanOrEqualTo(3);
    }
}