// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.Application.Services;

public interface IJobsTrackerService
{
    ValueTask<Guid> CreateJobListAsync(
        string name,
        string category,
        string username);

    ValueTask AddJobAsync(
        Guid jobListId,
        string title,
        DateTime due);

    ValueTask CompleteJobAsync(
        Guid jobListId,
        Guid jobId);
}