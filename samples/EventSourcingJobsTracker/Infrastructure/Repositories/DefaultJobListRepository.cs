// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Application.Infrastructure;
using EventSourcingJobsTracker.Core.Aggregates;
using EventSourcingJobsTracker.Infrastructure.Items;
using Microsoft.Azure.CosmosEventSourcing.Stores;

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
                    jobList.Username,
                    jobList.Category))
            .ToList();

        eventItems.Add(new JobListEventItem(
            jobList.AtomicEvent,
            jobList.Username,
            jobList.Category));

        await _eventStore.PersistAsync(eventItems);
    }
}