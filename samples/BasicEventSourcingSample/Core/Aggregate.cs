// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection.Metadata;
using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public abstract class Aggregate
{
    private List<IPersistedEvent> _events = new();
    private readonly List<IPersistedEvent> _unSavedEvents = new();

    public IReadOnlyList<IPersistedEvent> UnSavedEvents =>
        _unSavedEvents;

    protected void AddEvent(IPersistedEvent persistedEvent) =>
        _unSavedEvents.Add(persistedEvent);

    public void Apply(List<IPersistedEvent> persistedEvents)
    {
        if (!persistedEvents.Any())
        {
            return;
        }

        List<IPersistedEvent> orderedEvents = persistedEvents.OrderBy(x => x.OccuredUtc).ToList();

        orderedEvents.ForEach(Apply);

        _events = orderedEvents;
    }

    protected abstract void Apply(IPersistedEvent persistedEvent);
}