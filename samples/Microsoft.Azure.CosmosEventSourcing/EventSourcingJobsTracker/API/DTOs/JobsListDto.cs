// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.API.DTOs;

public record JobsListDto(string Id,
    string Username,
    string Category,
    DateTime Created);