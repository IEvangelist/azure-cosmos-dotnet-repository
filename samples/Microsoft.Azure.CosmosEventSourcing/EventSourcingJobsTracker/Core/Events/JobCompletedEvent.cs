// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingJobsTracker.Core.ValueObjects;
using Microsoft.Azure.CosmosEventSourcing.Events;

namespace EventSourcingJobsTracker.Core.Events;

public record JobCompletedEvent(
    Guid Id,
    string Title,
    JobListInfo JobListInfo) : DomainEvent;