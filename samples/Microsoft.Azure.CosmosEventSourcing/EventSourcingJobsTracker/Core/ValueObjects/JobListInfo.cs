// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.Core.ValueObjects;

public record JobListInfo(
    Guid Id,
    string Name,
    string Category,
    string Username);