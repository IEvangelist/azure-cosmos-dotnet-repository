// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcing;

internal class EventSourcingRepository<TEventStream> : IEventSourcingRepository<TEventStream> where TEventStream : EventSource
{
    private readonly IRepository<TEventStream> _repository;

    public EventSourcingRepository(IRepository<TEventStream> repository) =>
        _repository = repository;

    public async ValueTask PersistAsync(TEventStream record) =>
        await _repository.CreateAsync(record);

    public async ValueTask PersistAsync(IEnumerable<TEventStream> records) =>
        await _repository.CreateAsync(records);

    public ValueTask<IEnumerable<TEventStream>> ReadAsync(string partitionKey) =>
        _repository.GetAsync(x => x.PartitionKey == partitionKey);

    public IAsyncEnumerable<TEventStream> StreamAsync(string partitionKey)
    {
        throw new NotImplementedException();
    }
}