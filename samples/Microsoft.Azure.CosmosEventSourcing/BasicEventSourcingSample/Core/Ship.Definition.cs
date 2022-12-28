// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using CleanArchitecture.Exceptions;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;

namespace BasicEventSourcingSample.Core;

public partial class Ship : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public DateTime Commissioned { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public ShipStatus Status { get; private set; }
    public string? Port { get; private set; }
    public double? CargoWeight { get; private set; }

    public Ship(string name, DateTime commissioned, DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException<Ship>("A ship name must be provided");
        }

        AddEvent(new ShipEvents.ShipCreated(name, commissioned));
    }

    public void Dock(string port)
    {
        if (Status is not (ShipStatus.AtSea or ShipStatus.UnUsed))
        {
            throw new DomainException<Ship>($"A ship cannot dock when at status {Status}");
        }

        AddEvent(new ShipEvents.DockedInPort(Name, port));
    }

    public void StartLoading(string port)
    {
        if (Status is not (ShipStatus.Docked or ShipStatus.UnUsed))
        {
            throw new DomainException<Ship>($"A ship cannot start loading when at status {Status}");
        }

        if (Port != port)
        {
            throw new DomainException<Ship>(
                $"The ship cannot load at port {port} as it is docked at port {Port}");
        }

        AddEvent(new ShipEvents.Loading(Name, port));
    }

    public void FinishLoading(string port, double cargoWeight)
    {
        if (Status is not (ShipStatus.Loading or ShipStatus.UnUsed))
        {
            throw new DomainException<Ship>($"A ship cannot have finished loading when at status {Status}");
        }

        if (Port != port)
        {
            throw new DomainException<Ship>(
                $"The ship cannot finish loading at port {port} as it is docked at port {Port}");
        }

        AddEvent(new ShipEvents.Loaded(Name, port, cargoWeight));
    }

    public void Depart(string port)
    {
        if (Status is not (ShipStatus.AwaitingDeparture or ShipStatus.UnUsed))
        {
            throw new DomainException<Ship>($"A ship cannot depart when at status {Status}");
        }

        if (Port != port)
        {
            throw new DomainException<Ship>(
                $"The ship cannot depart {port} as it is awaiting departure at {Port}");
        }

        AddEvent(new ShipEvents.Departed(Name, port));
    }
}