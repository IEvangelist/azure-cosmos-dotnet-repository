// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public class Ship : Aggregate
{
    public string Name { get; }
    public DateTime Commissioned { get; }
    public DateTime CreatedAt { get; }

    public ShipStatus Status { get; private set; }

    public string? Port { get; private set; }

    public Ship(string name, DateTime commissioned, DateTime? createdAt = null)
    {
        Name = name;
        Commissioned = commissioned;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public void Dock(string port, DateTime occuredUtc) =>
        Apply(new ShipEvents.DockedInPort(Name, port, occuredUtc));

    public void StartLoading(string port, DateTime occuredUtc) =>
        Apply(new ShipEvents.Loading(Name, port, occuredUtc));

    private void Apply(ShipEvents.DockedInPort dockedInPort)
    {
        if (Status is not (ShipStatus.AtSea or ShipStatus.UnUsed))
        {
            throw new InvalidOperationException($"A ship cannot dock when at status {Status}");
        }

        Port = dockedInPort.Port;
        Status = ShipStatus.Docked;

        _events.Add(dockedInPort);
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

        _events.Add(loading);
    }

    private void Apply(ShipEvents.Loaded dockedInPort)
    {
        if (Status != ShipStatus.Loading)
        {
            throw new InvalidOperationException($"A ship cannot have finished loading when at status {Status}");
        }

        _events.Add(dockedInPort);
    }

    private void Apply(ShipEvents.Departed dockedInPort)
    {
        if (Status != ShipStatus.AwaitingDeparture)
        {
            throw new InvalidOperationException($"A ship cannot depart when at status {Status}");
        }

        _events.Add(dockedInPort);
    }

    protected override void HandleHydratedEvent(IPersistedEvent persistedEvent)
    {
        switch (persistedEvent)
        {
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
        }
    }
}