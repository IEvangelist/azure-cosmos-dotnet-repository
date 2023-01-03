// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Core.Events;
using EventSourcingJobsTracker.Core.ValueObjects;
using EventSourcingJobsTracker.Infrastructure.Items;
using EventSourcingJobsTracker.Infrastructure.Projections.Keys;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingJobsTracker.Infrastructure.Projections;

public class JobsListJobAddedProjection : IDomainEventProjection<JobAddedEvent, JobsListEventItem, JobsListProjectionKey>
{
    private readonly IWriteOnlyRepository<JobItem> _repository;

    public JobsListJobAddedProjection(IWriteOnlyRepository<JobItem> repository) =>
        _repository = repository;

    public async ValueTask HandleAsync(
        JobAddedEvent domainEvent,
        JobsListEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        (Guid guid, var title, DateTime due, JobListInfo? jobListInfo) = domainEvent;

        JobItem item = new(
            guid.ToString(),
            jobListInfo.Id.ToString(),
            title,
            due);

        await _repository.CreateAsync(
            item,
            cancellationToken);
    }
}