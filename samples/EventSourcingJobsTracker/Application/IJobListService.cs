// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.Application;

public interface IJobListService
{
    ValueTask<Guid> CreateJobList(
        string name,
        string category,
        string username);
}