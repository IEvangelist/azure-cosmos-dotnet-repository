// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Newtonsoft.Json;

namespace EventSourcingCustomerAccount.Items;

public class CustomerAccountEventItem : DefaultEventItem
{
    public CustomerAccountEventItem(
        string username,
        IDomainEvent domainEvent)
        : base(
            eventPayload: domainEvent,
            partitionKey: username)
    {
    }

    [JsonConstructor]
    public CustomerAccountEventItem(
        IDomainEvent eventPayload,
        string partitionKey) :
        base(eventPayload, partitionKey)
    {
    }
}