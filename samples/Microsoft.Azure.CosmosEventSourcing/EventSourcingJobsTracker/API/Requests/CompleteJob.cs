// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.API.Requests;

public record CompleteJob(
    Guid JobListId,
    Guid JobId);