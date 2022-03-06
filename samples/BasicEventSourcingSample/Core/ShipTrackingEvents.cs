// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing;

namespace BasicEventSourcingSample.Core;

public static class ShipEvents
{
    public record ShipCreated(string Name, DateTime Commissioned, DateTime OccuredUtc) : IPersistedEvent
    {
        public string EventName => nameof(ShipCreated);
    }

    public record DockedInPort(string Name, string Port, DateTime OccuredUtc) : IPersistedEvent
    {
        public string EventName => nameof(DockedInPort);
    }

    public record Loading(string Name, string Port, DateTime OccuredUtc) : IPersistedEvent
    {
        public string EventName => nameof(Loading);
    }

    public record Loaded(string Name, string Port, double CargoWeight, DateTime OccuredUtc) : IPersistedEvent
    {
        public string EventName => nameof(Loaded);
    }

    public record Departed(string Name, string Port, DateTime OccuredUtc) : IPersistedEvent
    {
        public string EventName => nameof(Departed);
    }
}