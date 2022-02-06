// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Core;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipRepository : IShipRepository
{
    private readonly IRepository<ShipCosmosItem> _cosmosRepository;
    private readonly IEventSourceRepository<ShipEventSource> _sourceRepository;

    public ShipRepository(
        IRepository<ShipCosmosItem> cosmosRepository,
        IEventSourceRepository<ShipEventSource> sourceRepository)
    {
        _cosmosRepository = cosmosRepository;
        _sourceRepository = sourceRepository;
    }

    public async ValueTask CreateShip(Ship ship) =>
        await _cosmosRepository.CreateAsync(ship.ToCosmosItem());

    public async ValueTask<Ship> FindAsync(string shipName)
    {
        ShipCosmosItem shipItem = await _cosmosRepository.GetAsync(shipName, nameof(ShipCosmosItem));
        Ship shipAggregate = shipItem.ToAggregate();

        IEnumerable<ShipEventSource> sourcedEvents = await _sourceRepository.ReadAsync(shipAggregate.Name);
        shipAggregate.Apply(sourcedEvents.Select(x => x.EventPayload).ToList());

        return shipAggregate;
    }

    public ValueTask SaveAsync(Ship ship)
    {
        IEnumerable<ShipEventSource> sourced =
            ship.UnSavedEvents.Select(x => new ShipEventSource(x, ship.Name));

        return _sourceRepository.PersistAsync(sourced);
    }

    public async Task<IEnumerable<string>> GetShipNamesAsync()
    {
        IEnumerable<ShipCosmosItem> ships = await _cosmosRepository.GetAsync(x => x.Type == nameof(ShipCosmosItem));
        return ships.Select(x => x.Name);
    }
}