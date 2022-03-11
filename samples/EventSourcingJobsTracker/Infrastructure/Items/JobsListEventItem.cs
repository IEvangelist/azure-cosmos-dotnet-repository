// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Newtonsoft.Json;

namespace EventSourcingJobsTracker.Infrastructure.Items;

public class JobsListEventItem : DefaultEventItem
{
    public JobsListEventItem(
        IDomainEvent domainEvent,
        Guid id) :
        base(domainEvent, id.ToString())
    {

    }

    [JsonConstructor]
    private JobsListEventItem(
        IDomainEvent eventPayload,
        string partitionKey) : base(eventPayload, partitionKey)
    {

    }
}