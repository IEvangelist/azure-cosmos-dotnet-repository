// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

/// <summary>
/// The class responsible for managing the persistence of the an <see cref="EventItem"/>
/// </summary>
/// <typeparam name="TEventItem"></typeparam>
public interface IEventStore<TEventItem> where TEventItem : EventItem
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
    /// Persists a set of <see cref="EventItem"/> items.
    /// </summary>
    /// <param name="aggregateRoot">The aggregate containing the events to persist.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a set of <see cref="EventItem"/> items.
    /// </summary>
    /// <param name="aggregateRoot">The aggregate containing the events to persist.</param>
    /// <param name="partitionKeyValue">The partition key value to use.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    ValueTask PersistAsync(
        IAggregateRoot aggregateRoot,
        string partitionKeyValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads all events for a given partition key.
    /// </summary>
    /// <param name="partitionKey">The value to use as the partition key in the query.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    /// <returns></returns>
    ValueTask<IEnumerable<TEventItem>> ReadAsync(
        string partitionKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads all events for a given partition key.
    /// </summary>
    /// <param name="partitionKey">The value to use as the partition key in the query.</param>
    /// <param name="predicate">A filter predicate to filter event on.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    /// <returns></returns>
    ValueTask<IEnumerable<TEventItem>> ReadAsync(
        string partitionKey,
        Expression<Func<TEventItem, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads all teh events for a given partition via streaming.
    /// </summary>
    /// <param name="partitionKey">The value to use as the partition key in the query.</param>
    /// <param name="chunkSize">The size in which the library will retrieve pages of events.</param>
    /// /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    /// <returns></returns>
    IAsyncEnumerable<TEventItem> StreamAsync(
        string partitionKey,
        int chunkSize = 25,
        CancellationToken cancellationToken = default);
}