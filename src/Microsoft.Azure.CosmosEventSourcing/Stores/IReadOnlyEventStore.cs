// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

/// <summary>
/// The interface responsible for reading collections of the <see cref="EventItem"/>'s
/// </summary>
public interface IReadOnlyEventStore<TEventItem> where TEventItem : EventItem
{
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
    /// Reads an <see cref="IAggregateRoot"/> by reading all it's events and applying them.
    /// </summary>
    /// <param name="partitionKey">The partitionKey used to read all the events.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    /// <typeparam name="TAggregateRoot">The type of the <see cref="IAggregateRoot"/> to read.</typeparam>
    /// <remarks>This looks for a static method on the given AggregateRoot type to call called Replay(...), it passes this method a list of <see cref="IDomainEvent"/>'s.</remarks>
    ValueTask<TAggregateRoot> ReadAggregateAsync<TAggregateRoot>(
        string partitionKey,
        CancellationToken cancellationToken = default)
        where TAggregateRoot : IAggregateRoot;

    /// <summary>
    /// Reads an <see cref="IAggregateRoot"/> created via a <see cref="IAggregateRootMapper{TAggregateRoot,TEventItem}"/>
    /// </summary>
    /// <param name="partitionKey">The partitionKey used to read all the events.</param>
    /// <param name="rootMapper">The rootMapper to use to map events to an <see cref="IAggregateRoot"/></param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    /// <typeparam name="TAggregateRoot">The type of the <see cref="IAggregateRoot"/> to read.</typeparam>
    ValueTask<TAggregateRoot> ReadAggregateAsync<TAggregateRoot>(
        string partitionKey,
        IAggregateRootMapper<TAggregateRoot, TEventItem> rootMapper,
        CancellationToken cancellationToken = default)
        where TAggregateRoot : IAggregateRoot;

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