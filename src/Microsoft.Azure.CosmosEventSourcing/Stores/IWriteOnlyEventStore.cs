// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Attributes;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

/// <summary>
/// The interface responsible for writing collections of the <see cref="EventItem"/>'s
/// </summary>
public interface IWriteOnlyEventStore<TEventItem> where TEventItem : EventItem
{
    /// <summary>
    /// Persists a set of <see cref="EventItem"/> items.
    /// </summary>
    /// <param name="items">The items to persist.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    ValueTask PersistAsync(
        IEnumerable<TEventItem> items,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists an <see cref="IAggregateRoot"/>
    /// </summary>
    /// <remarks>
    /// The <see cref="EventItemPartitionKeyAttribute"/> attribute must be set on a the property in the aggregate you wish to use for the partition key.
    /// </remarks>
    /// <remarks>
    /// This method uses <see cref="Activator"/> to build up the collection of <see cref="EventItem"/>'s.
    /// </remarks>
    /// <param name="aggregateRoot">The aggregate containing the events to persist.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists an <see cref="IAggregateRoot"/>
    /// </summary>
    /// <param name="aggregateRoot">The aggregate containing the events to persist.</param>
    /// <param name="mapper">The mapper to use to map the aggregate root to a collection of <see cref="EventItem"/></param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    ValueTask PersistAsync<TAggregateRoot>(
        TAggregateRoot aggregateRoot,
        IAggregateRootMapper<TAggregateRoot, TEventItem> mapper,
        CancellationToken cancellationToken = default)
        where TAggregateRoot : IAggregateRoot;

    /// <summary>
    /// Persists a set of <see cref="EventItem"/> items.
    /// </summary>
    /// <remarks>
    /// This method uses activator to build the EventItems.
    /// </remarks>
    /// <param name="aggregateRoot">The aggregate containing the events to persist.</param>
    /// <param name="partitionKeyValue">The partition key value to use.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        string partitionKeyValue,
        CancellationToken cancellationToken = default);
}