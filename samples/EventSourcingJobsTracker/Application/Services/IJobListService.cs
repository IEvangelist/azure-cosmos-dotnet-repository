// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.Application.Services;

public interface IJobListService
{
    ValueTask<Guid> CreateJobList(
        string name,
        string category,
        string username);

    ValueTask AddJob(
        Guid jobListId,
        string title,
        DateTime due);

    ValueTask CompleteJob(
        Guid jobListId,
        Guid jobId);
}