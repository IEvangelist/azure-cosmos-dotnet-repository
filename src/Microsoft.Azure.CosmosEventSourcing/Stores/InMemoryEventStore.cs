// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices.Context;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

internal class InMemoryEventStore<TEventItem>(
    IOptionsMonitor<CosmosEventSourcingOptions> optionsMonitor,
    IContextService contextService,
    IChangeFeedContainerProcessorProvider changeFeedContainerProcessorProvider) : IEventStore<TEventItem>
    where TEventItem : EventItem
{
    public ValueTask<IEnumerable<TEventItem>> ReadAsync(
        string partitionKey,
        CancellationToken cancellationToken = default) =>
        new(InMemoryStorage.ReadAllEvents<TEventItem>(partitionKey));

    public async ValueTask<TAggregateRoot> ReadAggregateAsync<TAggregateRoot>(
        string partitionKey,
        CancellationToken cancellationToken = default) where TAggregateRoot : IAggregateRoot
    {
        IEnumerable<TEventItem> events = await ReadAsync(
            partitionKey,
            cancellationToken);
        return events.Replay<TAggregateRoot, TEventItem>();
    }

    public async ValueTask<TAggregateRoot> ReadAggregateAsync<TAggregateRoot>(
        string partitionKey,
        IAggregateRootMapper<TAggregateRoot, TEventItem> rootMapper,
        CancellationToken cancellationToken = default) where TAggregateRoot : IAggregateRoot
    {
        IEnumerable<TEventItem> events = await ReadAsync(
            partitionKey,
            cancellationToken);
        return rootMapper.MapTo(events);
    }

    public async ValueTask<IEnumerable<TEventItem>> ReadAsync(
        string partitionKey,
        Expression<Func<TEventItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<TEventItem> events = await ReadAsync(
            partitionKey,
            cancellationToken);
        return events.Where(predicate.Compile());
    }

    public IAsyncEnumerable<TEventItem> StreamAsync(
        string partitionKey,
        int chunkSize = 25,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask PersistAsync(
        IEnumerable<TEventItem> items,
        CancellationToken cancellationToken = default)
    {
        var eventItems = items.ToList();
        if (eventItems is {Count: 0})
        {
            return;
        }

        if (optionsMonitor.CurrentValue.IsSequenceNumberingDisabled is false)
        {
            if (eventItems.Count(x => x.EventName is nameof(AtomicEvent)) is not 1)
            {
                throw new AtomicEventRequiredException();
            }
        }

        await InMemoryStorage.PersistAsync(
            eventItems.SetCorrelationId(contextService),
            eventItems.First().PartitionKey,
            changeFeedContainerProcessorProvider);
    }

    public async ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default) =>
        await PersistAsync(
            aggregateRoot,
            aggregateRoot.GetEventItemPartitionKeyValue(),
            cancellationToken);

    public async ValueTask PersistAsync<TAggregateRoot>(
        TAggregateRoot aggregateRoot,
        IAggregateRootMapper<TAggregateRoot, TEventItem> mapper,
        CancellationToken cancellationToken = default) where TAggregateRoot : IAggregateRoot
    {
        IEnumerable<TEventItem> events = mapper.MapFrom(aggregateRoot);
        await PersistAsync(
            events,
            cancellationToken);
    }

    public async ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) =>
        await PersistAsync(
            aggregateRoot.ToEventItems<TEventItem>(
                    partitionKeyValue,
                    optionsMonitor.CurrentValue.IsSequenceNumberingDisabled)
                .SetCorrelationId(contextService),
            cancellationToken);
}