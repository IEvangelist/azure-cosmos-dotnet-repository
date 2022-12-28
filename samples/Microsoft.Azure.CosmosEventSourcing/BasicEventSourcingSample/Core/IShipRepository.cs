// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Projections.Models;

namespace BasicEventSourcingSample.Core;

public interface IShipRepository
{
    ValueTask<Ship> FindAsync(string shipName);
    ValueTask SaveAsync(Ship ship);
    Task<IEnumerable<string>> GetShipNamesAsync();
    ValueTask<ShipInformation?> GetInformationAsync(string name);
}