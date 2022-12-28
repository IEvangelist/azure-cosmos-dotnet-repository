// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace EventSourcingJobsTracker.Core.Events;

public record JobListCreatedEvent(
    Guid Id,
    string Name,
    string Category,
    string Username) : DomainEvent;