// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Core.Events;
using EventSourcingJobsTracker.Infrastructure.Items;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingJobsTracker.Infrastructure.Projections;

public class UsersJobsListProjection : IDomainEventProjectionBuilder<JobListCreatedEvent, JobsListEventItem>
{
    private readonly IWriteOnlyRepository<JobsListReadItem> _repository;

    public UsersJobsListProjection(IWriteOnlyRepository<JobsListReadItem> repository) =>
        _repository = repository;

    public async ValueTask HandleAsync(
        JobListCreatedEvent domainEvent,
        JobsListEventItem eventSource,
        CancellationToken cancellationToken = default)
    {
        (Guid guid, string? name, string? category, string? username) = domainEvent;

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