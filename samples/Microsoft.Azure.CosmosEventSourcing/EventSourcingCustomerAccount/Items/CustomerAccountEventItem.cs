// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace EventSourcingCustomerAccount.Items;

public class CustomerAccountEventItem : EventItem
{
    public CustomerAccountEventItem(
        string username,
        DomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
        PartitionKey = username;
    }
}