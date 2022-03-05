// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Projections;
using BasicEventSourcingSample.Projections.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Infrastructure;

public class ShipRepository : IShipRepository
{
    private readonly IEventStore<ShipEventSource> _store;
    private readonly IRepository<ShipInformation> _shipInformationRepository;

    public ShipRepository(
        IEventStore<ShipEventSource> store,
        IRepository<ShipInformation> shipInformationRepository)
    {
        _store = store;
        _shipInformationRepository = shipInformationRepository;
    }

    public async ValueTask<Ship> FindAsync(string shipName)
    {
        IEnumerable<ShipEventSource> sourcedEvents = await _store.ReadAsync(shipName);
        Ship ship = Ship.Build(sourcedEvents.Select(x => x.EventPayload).ToList());
        return ship;
    }

    public ValueTask SaveAsync(Ship ship)
    {
        IEnumerable<ShipEventSource> sourced =
            ship.UnSavedEvents.Select(x => new ShipEventSource(x, ship.Name));

        return _store.PersistAsync(sourced);
    }

    public async Task<IEnumerable<string>> GetShipNamesAsync()
    {
        IEnumerable<ShipInformation>? all = await _shipInformationRepository
            .GetAsync(x => x.PartitionKey == nameof(ShipInformation));

        return all.Select(x => x.Name);
    }

    public async ValueTask<ShipInformation?> GetInformationAsync(string name)
    {
        try
        {
            return await _shipInformationRepository.GetAsync(name, nameof(ShipInformation));
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}