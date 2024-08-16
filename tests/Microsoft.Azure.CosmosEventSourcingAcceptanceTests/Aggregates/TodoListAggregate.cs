// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Entities;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Events;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Aggregates;

public class TodoListAggregate : AggregateRoot
{
    private readonly List<TodoItem> _items = [];

    public IReadOnlyList<TodoItem> Items => _items;

    public string Name { get; private set; } = null!;

    public TodoListAggregate(string name) =>
        AddEvent(new TodoListCreated(name));

    public void AddItem(string title) =>
        AddEvent(new TodoItemAdded(_items.Count + 1, title));

    public void CompleteItem(int id)
    {
        TodoItem item = _items.First(x => x.Id == id);

        AddEvent(new TodoItemCompleted(id, item.Title));
    }

    private TodoListAggregate()
    {

    }

    public static TodoListAggregate Replay(List<DomainEvent> domainEvents)
    {
        var a = new TodoListAggregate();
        a.Apply(domainEvents);
        return a;
    }

    protected override void Apply(DomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case TodoListCreated created:
                Apply(created);
                break;
            case TodoItemAdded added:
                Apply(added);
                break;
            case TodoItemCompleted completed:
                Apply(completed);
                break;
        }
    }

    private void Apply(TodoListCreated created) =>
        Name = created.Name;

    private void Apply(TodoItemAdded added) =>
        _items.Add(new TodoItem(added.Id, added.Title));

    private void Apply(TodoItemCompleted completed)
    {
        TodoItem item = _items.First(x => x.Id == completed.Id);
        item.Completed(completed.OccuredUtc);
    }
}