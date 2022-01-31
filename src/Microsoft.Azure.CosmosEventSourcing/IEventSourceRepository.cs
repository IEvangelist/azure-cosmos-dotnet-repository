// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing;

public interface IEventSourceRepository<TEventSource> where TEventSource : EventSource
{
    ValueTask PersistAsync(TEventSource record);

    ValueTask PersistAsync(IEnumerable<TEventSource> records);

    ValueTask<IEnumerable<TEventSource>> ReadAsync(string partitionKey);

    IAsyncEnumerable<TEventSource> StreamAsync(string partitionKey);
}