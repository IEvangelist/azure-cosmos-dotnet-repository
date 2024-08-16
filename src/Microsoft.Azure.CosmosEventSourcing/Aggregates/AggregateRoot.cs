// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;

namespace Microsoft.Azure.CosmosEventSourcing.Aggregates;

/// <inheritdoc />
public abstract class AggregateRoot : IAggregateRoot
{
    private List<DomainEvent> _events = [];
    private readonly List<DomainEvent> _newEvents = [];
    private AtomicEvent? _atomicEvent;

    /// <inheritdoc />
    public IReadOnlyList<DomainEvent> NewEvents =>
        _newEvents;

    /// <inheritdoc />
    public IReadOnlyList<DomainEvent> Events =>
        _events;


    /// <inheritdoc />
    public AtomicEvent AtomicEvent =>
        _atomicEvent ?? throw new AtomicEventRequiredException(GetType());

    /// <summary>
    /// Adds a new event to the <see cref="AggregateRoot"/>
    /// </summary>
    /// <param name="domainEvent">The <see cref="DomainEvent"/> to add.</param>
    /// <remarks>This should only be used when adding NEW events.</remarks>
    protected void AddEvent(DomainEvent domainEvent)
    {
        if (_atomicEvent is null)
        {
            CreateAtomicMarkerEvent();
        }

        if (!_newEvents.Any())
        {
            UpdateAtomicMarkerEvent();
        }

        DomainEvent evt = domainEvent with
        {
            EventId = Guid.NewGuid().ToString(),
            Sequence = _events.Count + 1,
            OccuredUtc = DateTime.UtcNow
        };

        _events.Add(evt);
        _newEvents.Add(evt);
        Apply(evt);
    }

    /// <summary>
    /// Applies a collection of <see cref="DomainEvent"/>'s
    /// </summary>
    /// <param name="domainEvents">The <see cref="DomainEvent"/>'s to apply</param>
    /// <remarks>This method is to be used when replaying a set of events.</remarks>
    protected void Apply(List<DomainEvent> domainEvents)
    {
        if (!domainEvents.Any())
        {
            throw new DomainEventsRequiredException(GetType());
        }

        var atomicEvent = domainEvents.SingleOrDefault(x => x is AtomicEvent) as AtomicEvent;

        _atomicEvent = atomicEvent ?? throw new AtomicEventRequiredException(GetType());
        domainEvents.Remove(atomicEvent);

        var orderedEvents = domainEvents
            .OrderBy(x => x.Sequence)
            .ToList();

        orderedEvents.ForEach(Apply);
        _events = orderedEvents;
    }

    /// <summary>
    /// A method which applies a specific <see cref="DomainEvent"/>
    /// </summary>
    /// <param name="domainEvent"></param>
    /// <example>
    /// Implementing your own aggregates apply methods.
    /// <code language="c#">
    /// <![CDATA[
    /// public class MyAggregate : AggregateRoot
    /// {
    ///     protected override void Apply(DomainEvent domainDomainEvent)
    ///     {
    ///         switch (domainDomainEvent)
    ///         {
    ///             case ShipEvents.ShipCreated created:
    ///                Apply(created);
    ///                break;
    ///             case ShipEvents.DockedInPort dockedInPort:
    ///                Apply(dockedInPort);
    ///                break;
    ///             case ShipEvents.Loading loading:
    ///                Apply(loading);
    ///                break;
    ///             case ShipEvents.Loaded loaded:
    ///                Apply(loaded);
    ///                break;
    ///             case ShipEvents.Departed departed:
    ///                Apply(departed);
    ///                break;
    ///             default:
    ///                throw new ArgumentOutOfRangeException(
    ///                    nameof(domainDomainEvent),
    ///                    $"No apply method found for {domainDomainEvent.GetType().Name}");
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    protected abstract void Apply(DomainEvent domainEvent);

    private void CreateAtomicMarkerEvent() => _atomicEvent = new AtomicEvent(nameof(AtomicEvent), Guid.NewGuid().ToString())
    {
        Sequence = int.MaxValue,
        OccuredUtc = DateTime.UtcNow
    };

    private void UpdateAtomicMarkerEvent()
    {
        if (_atomicEvent is null)
        {
            throw new AtomicEventRequiredException(GetType());
        }

        _atomicEvent = _atomicEvent with
        {
            OccuredUtc = DateTime.UtcNow
        };
    }
}