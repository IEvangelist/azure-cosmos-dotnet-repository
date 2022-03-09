// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

internal class DefaultEventStore<TEventItem> :
    IEventStore<TEventItem> where TEventItem : EventItem
{
    private readonly IRepository<TEventItem> _repository;

    public DefaultEventStore(IRepository<TEventItem> repository) =>
        _repository = repository;

    public async ValueTask PersistAsync(
        IEnumerable<TEventItem> records,
        CancellationToken cancellationToken = default) =>
        await _repository.CreateAsync(
            records,
            cancellationToken);

    public ValueTask<IEnumerable<TEventItem>> ReadAsync(string partitionKey,
        CancellationToken cancellationToken = default) =>
        _repository.GetAsync(
            x => x.PartitionKey == partitionKey,
            cancellationToken);

    public ValueTask<IEnumerable<TEventItem>> ReadAsync(
        string partitionKey,
        Expression<Func<TEventItem, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        _repository.GetAsync(
            predicate.Compose(
                x => x.PartitionKey == partitionKey,
                Expression.AndAlso),
            cancellationToken);

    public async IAsyncEnumerable<TEventItem> StreamAsync(
        string partitionKey,
        int chunkSize = 25,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? token = null;

        Expression<Func<TEventItem, bool>> expression = eventSource =>
            eventSource.PartitionKey == partitionKey;

        do
        {
            IPage<TEventItem> page = await _repository.PageAsync(
                expression,
                chunkSize,
                token,
                cancellationToken: cancellationToken);

            token = page.Continuation;

            foreach (TEventItem eventSource in page.Items)
            {
                yield return eventSource;
            }

        } while (token is not null);
    }
}