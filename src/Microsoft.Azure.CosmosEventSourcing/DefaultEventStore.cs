// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosEventSourcing;

internal class DefaultEventStore<TEventSource> :
    IEventStore<TEventSource> where TEventSource : EventSource
{
    private readonly IRepository<TEventSource> _repository;

    public DefaultEventStore(IRepository<TEventSource> repository) =>
        _repository = repository;

    public async ValueTask PersistAsync(
        IEnumerable<TEventSource> records,
        CancellationToken cancellationToken = default) =>
        await _repository.CreateAsync(
            records,
            cancellationToken);

    public ValueTask<IEnumerable<TEventSource>> ReadAsync(string partitionKey,
        CancellationToken cancellationToken = default) =>
        _repository.GetAsync(
            x => x.PartitionKey == partitionKey,
            cancellationToken);

    public ValueTask<IEnumerable<TEventSource>> ReadAsync(
        string partitionKey,
        Expression<Func<TEventSource, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        _repository.GetAsync(
            predicate.Compose(
                x => x.PartitionKey == partitionKey,
                Expression.AndAlso),
            cancellationToken);

    public async IAsyncEnumerable<TEventSource> StreamAsync(
        string partitionKey,
        int chunkSize = 25,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? token = null;

        Expression<Func<TEventSource, bool>> expression = eventSource =>
            eventSource.PartitionKey == partitionKey;

        do
        {
            IPage<TEventSource> page = await _repository.PageAsync(
                expression,
                chunkSize,
                token,
                cancellationToken: cancellationToken);

            token = page.Continuation;

            foreach (TEventSource eventSource in page.Items)
            {
                yield return eventSource;
            }

        } while (token is not null);
    }
}