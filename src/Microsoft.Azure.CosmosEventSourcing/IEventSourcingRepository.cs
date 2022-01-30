// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing;

public interface IEventSourcingRepository<TEventStream> where TEventStream : SourcedEvent
{
    ValueTask PersistAsync(TEventStream record);

    ValueTask PersistAsync(IEnumerable<TEventStream> records);

    ValueTask<IEnumerable<TEventStream>> ReadAsync(string partitionKey);

    IAsyncEnumerable<TEventStream> StreamAsync(string partitionKey);
}