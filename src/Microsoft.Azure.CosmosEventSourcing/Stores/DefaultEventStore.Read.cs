// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Microsoft.Azure.CosmosRepository.Paging;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

internal partial class DefaultEventStore<TEventItem>
{
    public ValueTask<IEnumerable<TEventItem>> ReadAsync(string partitionKey,
        CancellationToken cancellationToken = default) =>
        readOnlyRepository.GetAsync(new PartitionKey(partitionKey),
            cancellationToken);

    public async ValueTask<TAggregateRoot> ReadAggregateAsync<TAggregateRoot>(
        string partitionKey,
        CancellationToken cancellationToken = default)
        where TAggregateRoot : IAggregateRoot
    {
        IEnumerable<TEventItem> events = await readOnlyRepository.GetAsync(
            new PartitionKey(partitionKey),
            cancellationToken);

        var payloads = events
            .Select(x => x.DomainEvent)
            .ToList();

        Type type = typeof(TAggregateRoot);
        MethodInfo? method = type.GetMethod("Replay");

        return method is null
            ? throw new ReplayMethodNotDefinedException(type)
            : (TAggregateRoot)method.Invoke(null, new object[] { payloads })!;
    }

    public async ValueTask<TAggregateRoot> ReadAggregateAsync<TAggregateRoot>(
        string partitionKey,
        IAggregateRootMapper<TAggregateRoot, TEventItem> rootMapper,
        CancellationToken cancellationToken = default) where TAggregateRoot : IAggregateRoot
    {
        IEnumerable<TEventItem> events = await readOnlyRepository.GetAsync(
            new PartitionKey(partitionKey),
            cancellationToken);

        return rootMapper.MapTo(events);
    }

    public ValueTask<IEnumerable<TEventItem>> ReadAsync(
        string partitionKey,
        Expression<Func<TEventItem, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        readOnlyRepository.GetAsync(
            new PartitionKey(partitionKey),
            predicate,
            cancellationToken);

    public async IAsyncEnumerable<TEventItem> StreamAsync(
        string partitionKey,
        int chunkSize = 25,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? token = null;

        do
        {
            IPage<TEventItem> page = await readOnlyRepository.PageAsync(
                new PartitionKey(partitionKey),
                predicate: null,
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