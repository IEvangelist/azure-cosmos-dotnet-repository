// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Newtonsoft.Json;

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