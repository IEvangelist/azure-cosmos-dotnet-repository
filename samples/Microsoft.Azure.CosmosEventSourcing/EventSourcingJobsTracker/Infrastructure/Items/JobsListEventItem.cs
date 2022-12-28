// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Newtonsoft.Json;

namespace EventSourcingJobsTracker.Infrastructure.Items;

public class JobsListEventItem : EventItem
{
    public JobsListEventItem(
        DomainEvent domainDomainEvent,
        Guid id)

    {
        PartitionKey = id.ToString();
        DomainEvent = domainDomainEvent;
    }

    [JsonConstructor]
    private JobsListEventItem()
    {

    }
}