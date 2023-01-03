// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace BasicEventSourcingSample.Core;

public static class ShipEvents
{
    public record ShipCreated(string Name, DateTime Commissioned) : DomainEvent;

    public record DockedInPort(string Name, string Port) : DomainEvent;

    public record Loading(string Name, string Port) : DomainEvent;

    public record Loaded(string Name, string Port, double CargoWeight) : DomainEvent;

    public record Departed(string Name, string Port) : DomainEvent;
}