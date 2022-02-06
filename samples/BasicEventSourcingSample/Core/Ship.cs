// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public class Ship : Aggregate
{
    public string Name { get; private set; } = null!;
    public DateTime Commissioned { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ShipStatus Status { get; private set; }
    public string? Port { get; private set; }
    public double? CargoWeight { get; private set; }

    private Ship() { }

    public static Ship Build(List<IPersistedEvent> persistedEvents)
    {
        Ship ship = new();
        ship.Apply(persistedEvents);
        return ship;
    }

    public Ship(string name, DateTime commissioned, DateTime? createdAt = null) =>
        AddEvent(new ShipEvents.ShipCreated(name, commissioned, createdAt ?? DateTime.UtcNow));

    public void Dock(string port, DateTime occuredUtc) =>
        AddEvent(new ShipEvents.DockedInPort(Name, port, occuredUtc));

    public void StartLoading(string port, DateTime occuredUtc) =>
        AddEvent(new ShipEvents.Loading(Name, port, occuredUtc));

    public void FinishLoading(string port, double cargoWeight, DateTime occuredUtc) =>
        AddEvent(new ShipEvents.Loaded(Name, port, cargoWeight, occuredUtc));

    public void Depart(string port, DateTime occuredUtc) =>
        AddEvent(new ShipEvents.Departed(Name, port, occuredUtc));

    private void Apply(ShipEvents.ShipCreated shipCreated)
    {
        if (string.IsNullOrWhiteSpace(shipCreated.Name))
        {
            throw new InvalidOperationException("A ship name must be provided");
        }

        CreatedAt = shipCreated.OccuredUtc;
        Name = shipCreated.Name;
        Commissioned = shipCreated.Commissioned;
    }

    private void Apply(ShipEvents.DockedInPort dockedInPort)
    {
        if (Status is not (ShipStatus.AtSea or ShipStatus.UnUsed))
        {
            throw new InvalidOperationException($"A ship cannot dock when at status {Status}");
        }

        Port = dockedInPort.Port;
        Status = ShipStatus.Docked;
    }

    private void Apply(ShipEvents.Loading loading)
    {
        if (Status is not (ShipStatus.Docked or ShipStatus.UnUsed))
        {
            throw new InvalidOperationException($"A ship cannot start loading when at status {Status}");
        }

        if (Port != loading.Port)
        {
            throw new InvalidOperationException(
                $"The ship cannot load at port {loading.Port} as it is docked at port {Port}");
        }

        Status = ShipStatus.Loading;
    }

    private void Apply(ShipEvents.Loaded loaded)
    {
        if (Status is not (ShipStatus.Loading or ShipStatus.UnUsed))
        {
            throw new InvalidOperationException($"A ship cannot have finished loading when at status {Status}");
        }

        if (Port != loaded.Port)
        {
            throw new InvalidOperationException(
                $"The ship cannot finish loading at port {loaded.Port} as it is docked at port {Port}");
        }

        CargoWeight = loaded.CargoWeight;
        Status = ShipStatus.AwaitingDeparture;
    }

    private void Apply(ShipEvents.Departed departed)
    {
        if (Status is not (ShipStatus.AwaitingDeparture or ShipStatus.UnUsed))
        {
            throw new InvalidOperationException($"A ship cannot depart when at status {Status}");
        }

        if (Port != departed.Port)
        {
            throw new InvalidOperationException(
                $"The ship cannot depart {departed.Port} as it is awaiting departure at {Port}");
        }

        Status = ShipStatus.AtSea;
    }

    protected override void Apply(IPersistedEvent persistedEvent)
    {
        switch (persistedEvent)
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
                    nameof(persistedEvent),
                    $"No apply method found for {persistedEvent.GetType().Name}");
        }
    }
}