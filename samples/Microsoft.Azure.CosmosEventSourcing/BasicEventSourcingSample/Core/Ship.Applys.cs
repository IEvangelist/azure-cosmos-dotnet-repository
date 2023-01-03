// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace BasicEventSourcingSample.Core;

public partial class Ship
{
    private Ship() { }

    public static Ship Build(List<DomainEvent> persistedEvents)
    {
        Ship ship = new();
        ship.Apply(persistedEvents);
        return ship;
    }

    private void Apply(ShipEvents.ShipCreated shipCreated)
    {
        (var name, DateTime commissioned) = shipCreated;
        Name = name;
        Commissioned = commissioned;
    }

    private void Apply(ShipEvents.DockedInPort dockedInPort)
    {
        Port = dockedInPort.Port;
        Status = ShipStatus.Docked;
    }

    private void Apply(ShipEvents.Loading _) =>
        Status = ShipStatus.Loading;

    private void Apply(ShipEvents.Loaded loaded)
    {
        CargoWeight = loaded.CargoWeight;
        Status = ShipStatus.AwaitingDeparture;
    }

    private void Apply(ShipEvents.Departed _) =>
        Status = ShipStatus.AtSea;

    protected override void Apply(DomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ShipEvents.ShipCreated created:
                Apply(created);
                break;
            case ShipEvents.DockedInPort dockedInPort:
                Apply(dockedInPort);
                break;
            case ShipEvents.Loading loading:
                Apply(loading);
                break;
            case ShipEvents.Loaded loaded:
                Apply(loaded);
                break;
            case ShipEvents.Departed departed:
                Apply(departed);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(domainEvent),
                    $"No apply method found for {domainEvent.GetType().Name}");
        }
    }
}