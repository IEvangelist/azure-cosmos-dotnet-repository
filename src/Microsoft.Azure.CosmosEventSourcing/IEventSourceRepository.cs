// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing;

/// <summary>
/// The repository responsible for managing the persistence of the an <see cref="EventSource"/>
/// </summary>
/// <typeparam name="TEventSource"></typeparam>
public interface IEventSourceRepository<TEventSource> where TEventSource : EventSource
{
    /// <summary>
    /// Persists a set of <see cref="EventSource"/> records.
    /// </summary>
    /// <param name="records">The records to persist.</param>
    /// <returns></returns>
    ValueTask PersistAsync(IEnumerable<TEventSource> records);

    /// <summary>
    /// Reads all events for a given partition key.
    /// </summary>
    /// <param name="partitionKey">The value to use as the partition key in the query.</param>
    /// <returns></returns>
    ValueTask<IEnumerable<TEventSource>> ReadAsync(string partitionKey);

    /// <summary>
    /// Reads all teh events for a given partition via streaming.
    /// </summary>
    /// <param name="partitionKey">The value to use as the partition key in the query.</param>
    /// <param name="chunkSize">The size in which the library will retrieve pages of events.</param>
    /// <returns></returns>
    IAsyncEnumerable<TEventSource> StreamAsync(
        string partitionKey,
        int chunkSize = 25);
}