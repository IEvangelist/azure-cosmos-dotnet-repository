// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosEventSourcing.Events;

namespace BasicEventSourcingSample.Core;

public abstract class Aggregate
{
    private List<DomainEvent> _events = new();
    private readonly List<DomainEvent> _unSavedEvents = new();

    public IReadOnlyList<DomainEvent> UnSavedEvents =>
        _unSavedEvents;

    protected void AddEvent(DomainEvent domainEvent)
    {
        DomainEvent evt = domainEvent with
        {
            Sequence = _events.Count + 1,
            OccuredUtc = DateTime.UtcNow
        };

        _events.Add(evt);
        _unSavedEvents.Add(evt);
        Apply(evt);
    }

    protected void Apply(List<DomainEvent> domainEvents)
    {
        if (!domainEvents.Any())
        {
            return;
        }

        List<DomainEvent> orderedEvents = domainEvents.OrderBy(x => x.Sequence).ToList();

        orderedEvents.ForEach(Apply);

        _events = orderedEvents;
    }

    protected abstract void Apply(IDomainEvent domainEvent);
}