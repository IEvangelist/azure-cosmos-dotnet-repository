// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Projections.Models;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Projections;

public class ShipInformationProjections
{
    public class ShipCreatedHandler : IEventProjectionHandler<ShipEvents.ShipCreated>
    {
        private readonly IRepository<ShipInformation> _repository;

        public ShipCreatedHandler(IRepository<ShipInformation> repository) =>
            _repository = repository;

        public async ValueTask HandleAsync(
            ShipEvents.ShipCreated shipCreated,
            CancellationToken cancellationToken = default)
        {
            ShipInformation info = new(
                shipCreated.Name,
                shipCreated.Commissioned,
                shipCreated.OccuredUtc);

            await _repository.UpdateAsync(info, cancellationToken);
        }
    }

    public class ShipDockedHandler : IEventProjectionHandler<ShipEvents.DockedInPort>
    {
        private readonly IRepository<ShipInformation> _repository;

        public ShipDockedHandler(IRepository<ShipInformation> repository) =>
            _repository = repository;

        public async ValueTask HandleAsync(ShipEvents.DockedInPort dockedInPort,
            CancellationToken cancellationToken = default)
        {
            (string name, string port, _) = dockedInPort;

            ShipInformation shipInfo = await _repository.GetAsync(
                name,
                nameof(ShipInformation),
                cancellationToken);

            shipInfo.LatestPort = port;

            await _repository.UpdateAsync(shipInfo, cancellationToken);
        }
    }

    public class ShipLoadedHandler : IEventProjectionHandler<ShipEvents.Loaded>
    {
        private readonly IRepository<ShipInformation> _repository;

        public ShipLoadedHandler(IRepository<ShipInformation> repository) =>
            _repository = repository;

        public async ValueTask HandleAsync(
            ShipEvents.Loaded loaded,
            CancellationToken cancellationToken = default)
        {
            (string name, string port, double cargoWeight, _) = loaded;

            ShipInformation shipInfo = await _repository.GetAsync(
                name,
                nameof(ShipInformation),
                cancellationToken);

            shipInfo.LatestPort = port;
            shipInfo.LatestCargoWeight = cargoWeight;

            await _repository.UpdateAsync(shipInfo, cancellationToken);
        }
    }
}