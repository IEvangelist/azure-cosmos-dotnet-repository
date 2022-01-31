// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcing;

internal class EventSourceRepository<TEventSource> : IEventSourceRepository<TEventSource> where TEventSource : EventSource
{
    private readonly IRepository<TEventSource> _repository;

    public EventSourceRepository(IRepository<TEventSource> repository) =>
        _repository = repository;

    public async ValueTask PersistAsync(TEventSource record) =>
        await _repository.CreateAsync(record);

    public async ValueTask PersistAsync(IEnumerable<TEventSource> records) =>
        await _repository.CreateAsync(records);

    public ValueTask<IEnumerable<TEventSource>> ReadAsync(string partitionKey) =>
        _repository.GetAsync(x => x.PartitionKey == partitionKey);

    public IAsyncEnumerable<TEventSource> StreamAsync(string partitionKey)
    {
        throw new NotImplementedException();
    }
}