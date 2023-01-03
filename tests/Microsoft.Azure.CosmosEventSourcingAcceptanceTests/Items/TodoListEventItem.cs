// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;

public class TodoListEventItem : EventItem
{
    public TodoListEventItem(
        DomainEvent domainEvent,
        string name)
    {
        DomainEvent = domainEvent;
        PartitionKey = name;
    }
}