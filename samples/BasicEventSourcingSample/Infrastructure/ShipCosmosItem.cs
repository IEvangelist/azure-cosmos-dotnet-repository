// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipCosmosItem : FullItem
{
    public string Name { get; }

    public DateTime Commissioned { get; }

    public ShipCosmosItem(string name, DateTime commissioned)
    {
        Id = name;
        Name = name;
        Commissioned = commissioned;
    }
}