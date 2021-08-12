// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository
{
    /// <summary>
    /// This is the repository interface for any implementation of
    /// <typeparamref name="TItem"/>, exposing asynchronous C.R.U.D. functionality.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="IItem"/> implementation class type.</typeparam>
    /// <example>
    /// With DI, use .ctor injection to require any implementation of <see cref="IItem"/>:
    /// <code language="c#">
    /// <![CDATA[
    /// public class ConsumingService
    /// {
    ///     readonly IRepository<SomePoco> _pocoRepository;
    ///
    ///     public ConsumingService(IRepository<SomePoco> pocoRepository) =>
    ///         _pocoRepository = pocoRepository;
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public interface IRepository<TItem> where TItem : IItem
    {
        /// <summary>
        /// Gets the <see cref="IItem"/> implementation class instance as a <typeparamref name="TItem"/> that corresponds to the given <paramref name="id"/>.
        /// </summary>
        /// <remarks>
        /// If the typeof(<typeparamref name="TItem"/>).Name differs from the item.Type you're attempting to retrieve, null is returned.
        /// </remarks>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKeyValue">The partition key value if different than the <see cref="IItem.Id"/>.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask{TItem}"/> representing the <see cref="IItem"/> implementation class instance as a <typeparamref name="TItem"/>.</returns>
        ValueTask<TItem> GetAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="IItem"/> implementation class instance as a <typeparamref name="TItem"/> that corresponds to the given <paramref name="id"/>.
        /// </summary>
        /// <remarks>
        /// If the typeof(<typeparamref name="TItem"/>).Name differs from the item.Type you're attempting to retrieve, null is returned.
        /// </remarks>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKey">The <see cref="PartitionKey"/> value if different than the <see cref="IItem.Id"/>.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask{TItem}"/> representing the <see cref="IItem"/> implementation class instance as a <typeparamref name="TItem"/>.</returns>
        ValueTask<TItem> GetAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an <see cref="IEnumerable{TItem}"/> collection of <see cref="IItem"/>
        /// implementation classes that match the given <paramref name="predicate"/>.
        /// </summary>
        /// <remarks>
        /// If the typeof(<typeparamref name="TItem"/>).Name differs from the item.Type you're attempting to retrieve, the item is not returned.
        /// </remarks>
        /// <param name="predicate">The expression used for evaluating a matching item.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A collection of item instances who meet the <paramref name="predicate"/> condition.</returns>
        ValueTask<IEnumerable<TItem>> GetAsync(
            Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an <see cref="IEnumerable{TItem}"/> collection of <see cref="IItem" />
        /// by the given Cosmos SQL query
        /// </summary>
        /// <param name="query">The Cosmos SQL query</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A collection of item instances returned by the given <paramref name="query"/> Cosmos SQL query.</returns>
        ValueTask<IEnumerable<TItem>> GetByQueryAsync(
            string query,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an <see cref="IEnumerable{TItem}"/> collection of <see cref="IItem" />
        /// by the given Cosmos QueryDefinition
        /// </summary>
        /// <param name="queryDefinition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of item instances returned by the given <paramref name="queryDefinition"/> Cosmos SQL query.</returns>
        ValueTask<IEnumerable<TItem>> GetByQueryAsync(
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a cosmos item representing the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The item value to create.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask{TItem}"/> representing the <see cref="IItem"/> implementation class instance as a <typeparamref name="TItem"/>.</returns>
        ValueTask<TItem> CreateAsync(
            TItem value,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates one or more cosmos item(s) representing the given <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The item values to create.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A collection of created item instances.</returns>
        ValueTask<IEnumerable<TItem>> CreateAsync(
            IEnumerable<TItem> values,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the cosmos object that corresponds to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The item value to update.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask{TItem}"/> representing the <see cref="IItem"/> implementation class instance as a <typeparamref name="TItem"/>.</returns>
        ValueTask<TItem> UpdateAsync(
            TItem value,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The object to delete.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous delete operation.</returns>
        ValueTask DeleteAsync(
            TItem value,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKeyValue">The partition key value if different than the <see cref="IItem.Id"/>.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous delete operation.</returns>
        ValueTask DeleteAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKey">The <see cref="PartitionKey"/> value if different than the <see cref="IItem.Id"/>.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous delete operation.</returns>
        ValueTask DeleteAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="predicate">The expression used for evaluating a matching item.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>The number of elements in a sequence.</returns>
        ValueTask<int> CountAsync(Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default);
    }
}