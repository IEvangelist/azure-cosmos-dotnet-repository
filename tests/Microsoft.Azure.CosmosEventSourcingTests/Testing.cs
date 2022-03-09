// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosEventSourcing;

namespace Microsoft.Azure.CosmosEventSourcingTests;

public static class Testing
{
    public record SampleEvent(DateTime OccuredUtc) : IDomainEvent
    {
        public string EventName { get; } = nameof(SampleEvent);
    }

    public class SampleEventItem : EventItem
    {
        public SampleEventItem(
            IDomainEvent eventPayload,
            string partitionKey) : base(eventPayload, partitionKey)
        {

        }
    }
}