// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection.Metadata;
using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public abstract class Aggregate
{
    protected List<IPersistedEvent> _events = new();
    protected DateTime? _restoredAt;

    public IReadOnlyList<IPersistedEvent> Events =>
        _restoredAt.HasValue
            ? _events.Where(x => x.OccuredUtc < _restoredAt).ToList()
            : new List<IPersistedEvent>();

    public void ReHydrate(List<IPersistedEvent> persistedEvents)
    {
        if (!persistedEvents.Any())
        {
            return;
        }

        List<IPersistedEvent> orderedEvents = persistedEvents.OrderBy(x => x.OccuredUtc).ToList();

        orderedEvents.ForEach(HandleHydratedEvent);

        _events = orderedEvents;
        _restoredAt = orderedEvents.Last().OccuredUtc;
    }

    protected abstract void HandleHydratedEvent(IPersistedEvent persistedEvent);
}