// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Core;

namespace BasicEventSourcingSample.Infrastructure;

public static class ShipExtensions
{
    public static ShipCosmosItem ToCosmosItem(this Ship ship) => new(ship.Name, ship.Commissioned);

    public static Ship ToAggregate(this ShipCosmosItem cosmosItem) =>
        new(cosmosItem.Name, cosmosItem.Commissioned, cosmosItem.CreatedTimeUtc ?? throw new Exception());
}