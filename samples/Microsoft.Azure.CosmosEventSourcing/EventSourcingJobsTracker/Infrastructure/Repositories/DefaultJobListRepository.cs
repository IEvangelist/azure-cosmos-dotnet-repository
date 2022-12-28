// Copyright (c) David Pine. All rights reserved.
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
    private readonly IEventStore<JobsListEventItem> _eventStore;

    public DefaultJobListRepository(IEventStore<JobsListEventItem> eventStore) =>
        _eventStore = eventStore;

    public async ValueTask SaveAsync(JobsList jobList)
    {
        var eventItems = jobList
            .NewEvents
            .Select(evt =>
                new JobsListEventItem(
                    evt,
                    jobList.Id))
            .ToList();

        eventItems.Add(new JobsListEventItem(
            jobList.AtomicEvent,
            jobList.Id));

        await _eventStore.PersistAsync(eventItems);
    }

    public async ValueTask<JobsList> ReadAsync(Guid jobListId)
    {
        List<JobsListEventItem> events = await _eventStore
            .ReadAsync(jobListId.ToString())
            .ToListAsync();

        if (events is { Count: 0 })
        {
            throw new ResourceNotFoundException<JobsList>(
                $"There is no job list with the ID {jobListId}");
        }

        return JobsList.Replay(events.Select(x => x.DomainEvent).ToList());
    }
}