// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
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

    [JsonConstructor]
    private TodoListEventItem()
    {

    }
}