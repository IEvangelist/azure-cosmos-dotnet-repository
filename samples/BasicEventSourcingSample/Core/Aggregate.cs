// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public abstract class Aggregate
{
    private List<IDomainEvent> _events = new();
    private readonly List<IDomainEvent> _unSavedEvents = new();

    public IReadOnlyList<IDomainEvent> UnSavedEvents =>
        _unSavedEvents;

    protected void AddEvent(IDomainEvent domainEvent)
    {
        _unSavedEvents.Add(domainEvent);
        Apply(domainEvent);
    }

    public void Apply(List<IDomainEvent> persistedEvents)
    {
        if (!persistedEvents.Any())
        {
            return;
        }

        List<IDomainEvent> orderedEvents = persistedEvents.OrderBy(x => x.OccuredUtc).ToList();

        orderedEvents.ForEach(Apply);

        _events = orderedEvents;
    }

    protected abstract void Apply(IDomainEvent domainEvent);
}