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

    public async ValueTask<Guid> CreateJobList(
        string name,
        string category,
        string username)
    {
        JobsList jobList = new(name, category, username);

        await _jobListRepository.SaveAsync(jobList);

        return jobList.Id;
    }

    public async ValueTask AddJob(
        Guid jobListId,
        string title,
        DateTime due)
    {
        JobsList jobsList = await _jobListRepository.ReadAsync(jobListId);

        jobsList.AddJob(title, due);

        await _jobListRepository.SaveAsync(jobsList);
    }

    public async ValueTask CompleteJob(Guid jobListId, Guid jobId)
    {
        JobsList jobsList = await _jobListRepository.ReadAsync(jobListId);

        jobsList.CompleteJob(jobId);

        await _jobListRepository.SaveAsync(jobsList);
    }
}