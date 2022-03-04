// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosEventSourcing;

namespace Microsoft.Azure.CosmosEventSourcingTests;

public static class Testing
{
    public record SampleEvent(DateTime OccuredUtc) : IPersistedEvent
    {
        public string EventName { get; } = nameof(SampleEvent);
    }

    public class SampleEventSource : EventSource
    {
        public SampleEventSource(
            IPersistedEvent eventPayload,
            string partitionKey) : base(eventPayload, partitionKey)
        {

        }
    }
}