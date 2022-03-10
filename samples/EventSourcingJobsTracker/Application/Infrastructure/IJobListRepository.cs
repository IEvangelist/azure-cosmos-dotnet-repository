// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Core.Aggregates;

namespace EventSourcingJobsTracker.Application.Infrastructure;

public interface IJobListRepository
{
    ValueTask SaveAsync(JobsList jobList);
}