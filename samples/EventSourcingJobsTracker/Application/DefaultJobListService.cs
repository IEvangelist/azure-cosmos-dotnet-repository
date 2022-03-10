// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Application.Infrastructure;
using EventSourcingJobsTracker.Core.Aggregates;

namespace EventSourcingJobsTracker.Application;

public class DefaultJobListService : IJobListService
{
    private readonly IJobListRepository _jobListRepository;

    public DefaultJobListService(IJobListRepository jobListRepository) =>
        _jobListRepository = jobListRepository;

    public async ValueTask<Guid> CreateJobList(string name, string category, string username)
    {
        JobsList jobList = new(name, category, username);

        await _jobListRepository.SaveAsync(jobList);

        return jobList.Id;
    }
}