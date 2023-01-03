// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Infrastructure;
using BasicEventSourcingSample.Projections.Models;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Projections;

public record ShipInformationProjectionKey : IProjectionKey;

public class ShipInformationProjections
{
    public class ShipCreated : IDomainEventProjection<ShipEvents.ShipCreated, ShipEventItem, ShipInformationProjectionKey>
    {
        private readonly IRepository<ShipInformation> _repository;

        public ShipCreated(IRepository<ShipInformation> repository) =>
            _repository = repository;

        public async ValueTask HandleAsync(
            ShipEvents.ShipCreated domainEvent,
            ShipEventItem eventItem,
            CancellationToken cancellationToken = default)
        {
            ShipInformation info = new(
                domainEvent.Name,
                domainEvent.Commissioned,
                domainEvent.OccuredUtc);

            await _repository.UpdateAsync(info, cancellationToken: cancellationToken);
        }
    }

    public class ShipDocked : IDomainEventProjection<ShipEvents.DockedInPort, ShipEventItem, ShipInformationProjectionKey>
    {
        private readonly IRepository<ShipInformation> _repository;

        public ShipDocked(IRepository<ShipInformation> repository) =>
            _repository = repository;

        public async ValueTask HandleAsync(
            ShipEvents.DockedInPort domainEvent,
            ShipEventItem eventItem,
            CancellationToken cancellationToken = default)
        {
            (var name, var port) = domainEvent;

            ShipInformation shipInfo = await _repository.GetAsync(
                name,
                nameof(ShipInformation),
                cancellationToken);

            shipInfo.LatestPort = port;

            await _repository.UpdateAsync(shipInfo, cancellationToken: cancellationToken);
        }
    }

    public class ShipLoaded : IDomainEventProjection<ShipEvents.Loaded, ShipEventItem, ShipInformationProjectionKey>
    {
        private readonly IRepository<ShipInformation> _repository;

        public ShipLoaded(IRepository<ShipInformation> repository) =>
            _repository = repository;

        public async ValueTask HandleAsync(
            ShipEvents.Loaded domainEvent,
            ShipEventItem eventItem,
            CancellationToken cancellationToken = default)
        {
            (var name, var port, var cargoWeight) = domainEvent;

            ShipInformation shipInfo = await _repository.GetAsync(
                name,
                nameof(ShipInformation),
                cancellationToken);

            shipInfo.LatestPort = port;
            shipInfo.LatestCargoWeight = cargoWeight;

            await _repository.UpdateAsync(shipInfo, cancellationToken: cancellationToken);
        }
    }
}