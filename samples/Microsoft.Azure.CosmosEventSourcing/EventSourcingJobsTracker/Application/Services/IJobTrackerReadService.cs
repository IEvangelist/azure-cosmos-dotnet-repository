// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.API.DTOs;

namespace EventSourcingJobsTracker.Application.Services;

public interface IJobTrackerReadService
{
    ValueTask<JobsListDto?> FindJobsListAsync(
        Guid id,
        string username);

    ValueTask<IEnumerable<JobDto>> FindJobsForJobsListAsync(Guid jobListId);
    Task<IEnumerable<JobsListDto>> FindJobsListAsync(string username);
}