// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public abstract class Aggregate
{
    private List<DefaultDomainEvent> _events = new();
    private readonly List<DefaultDomainEvent> _unSavedEvents = new();

    public IReadOnlyList<DefaultDomainEvent> UnSavedEvents =>
        _unSavedEvents;

    protected void AddEvent(DefaultDomainEvent domainEvent)
    {
        DefaultDomainEvent evt = domainEvent with
        {
            Sequence = _events.Count + 1,
            OccuredUtc = DateTime.UtcNow
        };

        _events.Add(evt);
        _unSavedEvents.Add(evt);
        Apply(evt);
    }

    protected void Apply(List<DefaultDomainEvent> domainEvents)
    {
        if (!domainEvents.Any())
        {
            return;
        }

        List<DefaultDomainEvent> orderedEvents = domainEvents.OrderBy(x => x.Sequence).ToList();

        orderedEvents.ForEach(Apply);

        _events = orderedEvents;
    }

    protected abstract void Apply(IDomainEvent domainEvent);
}