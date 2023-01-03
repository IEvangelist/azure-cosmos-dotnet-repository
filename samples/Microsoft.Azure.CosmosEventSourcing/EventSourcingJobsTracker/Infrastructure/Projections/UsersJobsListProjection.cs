// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Core.Events;
using EventSourcingJobsTracker.Infrastructure.Items;
using EventSourcingJobsTracker.Infrastructure.Projections.Keys;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingJobsTracker.Infrastructure.Projections;

public class UsersJobsListProjection : IDomainEventProjection<JobListCreatedEvent, JobsListEventItem, JobsListProjectionKey>
{
    private readonly IWriteOnlyRepository<JobsListReadItem> _repository;

    public UsersJobsListProjection(IWriteOnlyRepository<JobsListReadItem> repository) =>
        _repository = repository;

    public async ValueTask HandleAsync(
        JobListCreatedEvent domainEvent,
        JobsListEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        (Guid guid, var name, var category, var username) = domainEvent;

        JobsListReadItem readItem = new(
            guid.ToString(),
            name,
            username,
            category);

        await _repository.CreateAsync(
            readItem,
            cancellationToken);
    }
}