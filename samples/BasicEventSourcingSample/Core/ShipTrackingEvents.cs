// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public static class ShipEvents
{
    public record ShipCreated(string Name, DateTime Commissioned) : DefaultDomainEvent;

    public record DockedInPort(string Name, string Port) : DefaultDomainEvent;

    public record Loading(string Name, string Port) : DefaultDomainEvent;

    public record Loaded(string Name, string Port, double CargoWeight) : DefaultDomainEvent;

    public record Departed(string Name, string Port) : DefaultDomainEvent;
}