// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcingTests;

public static class Testing
{
    public record SampleEvent(DateTime OccuredUtc) : DomainEvent;

    public class SampleEventItem : EventItem
    {
        public SampleEventItem(
            IDomainEvent eventPayload,
            string partitionKey) : base(eventPayload, partitionKey)
        {

        }
    }
}