// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Newtonsoft.Json;

namespace EventSourcingCustomerAccount.Items;

public class CustomerAccountEventItem : EventItem
{
    public CustomerAccountEventItem(
        DomainEvent domainEvent,
        string username)
    {
        DomainEvent = domainEvent;
        PartitionKey = username;

    }
}