// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Core;
using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipRepository : IShipRepository
{
    private readonly IRepository<ShipCosmosItem> _cosmosRepository;

    public ShipRepository(IRepository<ShipCosmosItem> cosmosRepository) =>
        _cosmosRepository = cosmosRepository;

    public async ValueTask CreateShip(Ship ship) =>
        await _cosmosRepository.CreateAsync(ship.ToCosmosItem());
}