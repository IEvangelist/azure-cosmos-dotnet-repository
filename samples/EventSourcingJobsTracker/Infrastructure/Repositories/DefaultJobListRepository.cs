// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using CleanArchitecture.Exceptions;
using EventSourcingJobsTracker.Application.Infrastructure;
using EventSourcingJobsTracker.Core.Aggregates;
using EventSourcingJobsTracker.Infrastructure.Items;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosRepository.Extensions;

namespace EventSourcingJobsTracker.Infrastructure.Repositories;

public class DefaultJobListRepository : IJobListRepository
{
    private readonly IEventStore<JobListEventItem> _eventStore;

    public DefaultJobListRepository(IEventStore<JobListEventItem> eventStore) =>
        _eventStore = eventStore;

    public async ValueTask SaveAsync(JobsList jobList)
    {
        List<JobListEventItem> eventItems = jobList
            .NewEvents
            .Select(evt =>
                new JobListEventItem(
                    evt,
                    jobList.Id))
            .ToList();

        eventItems.Add(new JobListEventItem(
            jobList.AtomicEvent,
            jobList.Id));

        await _eventStore.PersistAsync(eventItems);
    }

    public async ValueTask<JobsList> ReadAsync(Guid jobListId)
    {
        List<JobListEventItem> events = await _eventStore
            .ReadAsync(jobListId.ToString())
            .ToListAsync();

        if (events is {Count: 0})
        {
            throw new ResourceNotFoundException<JobsList>(
                $"There is no job list with the ID {jobListId}");
        }

        return JobsList.Replay(events.Select(x => x.DomainEventPayload).ToList());
    }
}