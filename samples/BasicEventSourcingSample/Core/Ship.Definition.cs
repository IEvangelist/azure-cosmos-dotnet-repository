// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public partial class Ship : Aggregate
{
    public string Name { get; private set; } = null!;
    public DateTime Commissioned { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ShipStatus Status { get; private set; }
    public string? Port { get; private set; }
    public double? CargoWeight { get; private set; }

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
}