// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.API.DTOs;

public record JobDto(
    string Id,
    string Title,
    DateTime Due,
    DateTime? CompletedAt = null)

{
    public bool IsComplete =>
        CompletedAt is not null;
}