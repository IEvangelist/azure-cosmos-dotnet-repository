// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Core.Events;
using EventSourcingJobsTracker.Core.ValueObjects;
using EventSourcingJobsTracker.Infrastructure.Items;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingJobsTracker.Infrastructure.Projections;

public class JobsListJobCompletedProjection : IDomainEventProjectionBuilder<JobCompletedEvent, JobsListEventItem>
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
        (Guid id, string? _, JobListInfo? jobListInfo) = domainEvent;

        JobItem item = await _repository.GetAsync(
            id.ToString(),
            jobListInfo.Id.ToString(),
            cancellationToken);

        item.Complete(domainEvent.OccuredUtc);

        await _repository.UpdateAsync(
            item,
            cancellationToken);
    }
}