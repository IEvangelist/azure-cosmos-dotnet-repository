// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.API.DTOs;
using EventSourcingJobsTracker.Infrastructure.Items;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingJobsTracker.Application.Services;

public class DefaultJobTrackerReadService : IJobTrackerReadService
{
    private readonly IReadOnlyRepository<JobsListReadItem> _jobsListRepository;
    private readonly IReadOnlyRepository<JobItem> _jobsRepository;

    public DefaultJobTrackerReadService(
        IReadOnlyRepository<JobsListReadItem> jobsListRepository,
        IReadOnlyRepository<JobItem> jobsRepository)
    {
        _jobsListRepository = jobsListRepository;
        _jobsRepository = jobsRepository;
    }

    public async ValueTask<JobsListDto?> FindJobsListAsync(
        Guid id,
        string username)
    {
        JobsListReadItem? jobsList = await _jobsListRepository.TryGetAsync(
            id.ToString(),
            username);

        return jobsList is null
            ? null
            : new JobsListDto(
                jobsList.Id,
                jobsList.Username,
                jobsList.Category,
                jobsList.CreatedTimeUtc!.Value);
    }

    public async ValueTask<IEnumerable<JobDto>> FindJobsForJobsListAsync(Guid jobListId)
    {
        IEnumerable<JobItem> jobs = await _jobsRepository.GetAsync(x =>
            x.PartitionKey == jobListId.ToString());

        return jobs.Select(x => new JobDto(
            x.Id,
            x.Title,
            x.Due,
            x.CompletedAt));
    }

    public async Task<IEnumerable<JobsListDto>> FindJobsListAsync(string username)
    {
        IEnumerable<JobsListReadItem> jobLists = await _jobsListRepository.GetAsync(x =>
            x.PartitionKey == username);

        return jobLists.Select(x => new JobsListDto(
            x.Id,
            x.Username,
            x.Category,
            x.CreatedTimeUtc!.Value));
    }
}