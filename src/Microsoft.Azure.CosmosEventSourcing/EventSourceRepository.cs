// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosEventSourcing;

internal class EventSourceRepository<TEventSource> : IEventSourceRepository<TEventSource> where TEventSource : EventSource
{
    private readonly IRepository<TEventSource> _repository;

    public EventSourceRepository(IRepository<TEventSource> repository) =>
        _repository = repository;

    public async ValueTask PersistAsync(IEnumerable<TEventSource> records) =>
        await _repository.CreateAsync(records);

    public ValueTask<IEnumerable<TEventSource>> ReadAsync(string partitionKey) =>
        _repository.GetAsync(x => x.PartitionKey == partitionKey);

    public async IAsyncEnumerable<TEventSource> StreamAsync(
        string partitionKey,
        int chunkSize = 25)
    {
        string? token = null;

        Expression<Func<TEventSource, bool>> expression = eventSource =>
            eventSource.PartitionKey == partitionKey;

        while (token is not null)
        {
            IPage<TEventSource> page = await _repository.PageAsync(
                expression,
                chunkSize,
                token);

            token = page.Continuation;

            foreach (TEventSource eventSource in page.Items)
            {
                yield return eventSource;
            }
        }
    }
}