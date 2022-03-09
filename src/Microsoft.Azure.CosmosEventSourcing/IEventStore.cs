// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosEventSourcing;

/// <summary>
/// The class responsible for managing the persistence of the an <see cref="EventItem"/>
/// </summary>
/// <typeparam name="TEventItem"></typeparam>
public interface IEventStore<TEventItem> where TEventItem : EventItem
{
    /// <summary>
    /// Persists a set of <see cref="EventItem"/> records.
    /// </summary>
    /// <param name="records">The records to persist.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this async request.</param>
    /// <returns></returns>
    ValueTask PersistAsync(
        IEnumerable<TEventItem> records,
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