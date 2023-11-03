// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Exceptions;
using Microsoft.Azure.CosmosEventSourcing.Extensions;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

internal partial class DefaultEventStore<TEventItem>
{
    public async ValueTask PersistAsync(
        IEnumerable<TEventItem> items,
        CancellationToken cancellationToken = default)
    {
        var eventItems = items.ToList();
        if (eventItems is { Count: 0 })
        {
            return;
        }

        if (_optionsMonitor.CurrentValue.IsSequenceNumberingDisabled is false)
        {
            if (eventItems.Count(x => x.EventName is nameof(AtomicEvent)) is not 1)
            {
                throw new AtomicEventRequiredException();
            }
        }

        await batchRepository.UpdateAsBatchAsync(
            eventItems.SetCorrelationId(contextService),
            cancellationToken);
    }

    public async ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default) =>
        await PersistAsync(
            aggregateRoot,
            aggregateRoot.GetEventItemPartitionKeyValue(),
            cancellationToken);

    public ValueTask PersistAsync<TAggregateRoot>(
        TAggregateRoot aggregateRoot,
        IAggregateRootMapper<TAggregateRoot, TEventItem> mapper,
        CancellationToken cancellationToken = default)
        where TAggregateRoot : IAggregateRoot =>
        batchRepository.UpdateAsBatchAsync(
            mapper.MapFrom(aggregateRoot).SetCorrelationId(contextService),
            cancellationToken);

    public async ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) =>
        await PersistAsync(
            aggregateRoot.ToEventItems<TEventItem>(
                    partitionKeyValue,
                    _optionsMonitor.CurrentValue.IsSequenceNumberingDisabled)
                .SetCorrelationId(contextService),
            cancellationToken);
}