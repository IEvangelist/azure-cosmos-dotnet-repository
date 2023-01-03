// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.API.Requests;

public record CreateJobList(
    string Name,
    string Category,
    string Username);