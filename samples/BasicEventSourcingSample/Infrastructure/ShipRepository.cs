// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Core;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipRepository : IShipRepository
{
    private readonly IRepository<ShipCosmosItem> _cosmosRepository;
    private readonly IEventSourcingRepository<SourcedShipEvent> _sourcingRepository;

    public ShipRepository(
        IRepository<ShipCosmosItem> cosmosRepository,
        IEventSourcingRepository<SourcedShipEvent> sourcingRepository)
    {
        _cosmosRepository = cosmosRepository;
        _sourcingRepository = sourcingRepository;
    }

    public async ValueTask CreateShip(Ship ship) =>
        await _cosmosRepository.CreateAsync(ship.ToCosmosItem());

    public async ValueTask<Ship> FindAsync(string shipName)
    {
        ShipCosmosItem shipItem = await _cosmosRepository.GetAsync(shipName, nameof(ShipCosmosItem));
        Ship shipAggregate = shipItem.ToAggregate();

        IEnumerable<SourcedShipEvent> sourcedEvents = await _sourcingRepository.ReadAsync(shipAggregate.Name);
        shipAggregate.ReHydrate(sourcedEvents.Select(x => x.Event).ToList());

        return shipAggregate;
    }

    public ValueTask SaveAsync(Ship ship)
    {
        IEnumerable<SourcedShipEvent> sourced = ship.Events.Select(x => new SourcedShipEvent(x, ship.Name));
        return _sourcingRepository.PersistAsync(sourced);
    }

    public async Task<IEnumerable<string>> GetShipNamesAsync()
    {
        IEnumerable<ShipCosmosItem> ships = await _cosmosRepository.GetAsync(x => x.Type == nameof(ShipCosmosItem));
        return ships.Select(x => x.Name);
    }
}