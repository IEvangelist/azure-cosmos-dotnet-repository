// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.API.Requests;

public record CreateJob(
    Guid JobListId,
    string Title,
    DateTime Due);