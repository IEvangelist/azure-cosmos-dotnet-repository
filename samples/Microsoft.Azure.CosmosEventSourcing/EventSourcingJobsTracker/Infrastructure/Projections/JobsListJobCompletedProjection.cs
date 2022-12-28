// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Core.Events;
using EventSourcingJobsTracker.Core.ValueObjects;
using EventSourcingJobsTracker.Infrastructure.Items;
using EventSourcingJobsTracker.Infrastructure.Projections.Keys;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingJobsTracker.Infrastructure.Projections;

public class JobsListJobCompletedProjection : IDomainEventProjection<JobCompletedEvent, JobsListEventItem, JobsListProjectionKey>
{
    private readonly IRepository<JobItem> _repository;

    public JobsListJobCompletedProjection(IRepository<JobItem> repository)
    {
        _repository = repository;
    }

    public async ValueTask HandleAsync(
        JobCompletedEvent domainEvent,
        JobsListEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        (Guid id, var _, JobListInfo? jobListInfo) = domainEvent;

        JobItem item = await _repository.GetAsync(
            id.ToString(),
            jobListInfo.Id.ToString(),
            cancellationToken);

        item.Complete(domainEvent.OccuredUtc);

        await _repository.UpdateAsync(
            item,
            cancellationToken: cancellationToken);
    }
}