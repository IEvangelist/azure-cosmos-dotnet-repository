// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace EventSourcingJobsTracker.Infrastructure.Items;

public class JobListEventItem : DefaultEventItem
{
    public JobListEventItem(
        IDomainEvent domainEvent,
        string username,
        string category) :
        base(domainEvent, $"{username}#{category}")
    {

    }

    public JobListEventItem(
        AtomicEvent domainEvent,
        string username,
        string category) :
        base(domainEvent, $"{username}#{category}")
    {

    }
}