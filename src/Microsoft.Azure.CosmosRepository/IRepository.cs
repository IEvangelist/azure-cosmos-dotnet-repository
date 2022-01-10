// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Paging;

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
        /// <param name="ignoreEtag">When TItem implements IItemWithEtag the etag will be verified on all updates. Setting this flag to true indicates that the etag should be ignored.</param>
        /// <returns>A <see cref="ValueTask{TItem}"/> representing the <see cref="IItem"/> implementation class instance as a <typeparamref name="TItem"/>.</returns>
        ValueTask<TItem> UpdateAsync(
            TItem value,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false);

        /// <summary>
        /// Updates one or more cosmos item(s) representing the given <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The item values to update.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <param name="ignoreEtag">When TItem implements IItemWithEtag the etag will be verified on all updates. Setting this flag to true indicates that the etag should be ignored.</param>
        /// <returns>A collection of updated item instances.</returns>
        ValueTask<IEnumerable<TItem>> UpdateAsync(
            IEnumerable<TItem> values,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false);


        /// <summary>
        /// Updates the given cosmos item using the provided and supported patch operations.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKeyValue">The partition key value if different than the <see cref="IItem.Id"/>.</param>
        /// <param name="builder">The <see cref="IPatchOperationBuilder{TItem}"/> that will define the update operations to perform.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <param name="etag">Indicate to set IfMatchEtag in the ItemRequestOptions in the underlying Cosmos call. This requires TItem to implement the IItemWithEtag interface.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask UpdateAsync(
            string id,
            Action<IPatchOperationBuilder<TItem>> builder,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default,
            string etag = default);

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
        /// Queries cosmos DB to see if an item exists.
        /// </summary>
        /// <remarks>This method performs a point read to decide whether or not an item exists.</remarks>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKeyValue">The partition key value if different than the <see cref="IItem.Id"/>.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous exists operation.</returns>
        ValueTask<bool> ExistsAsync(string id, string partitionKeyValue = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Queries cosmos DB to see if an item exists.
        /// </summary>
        /// <remarks>This method performs a point read to decide whether or not an item exists.</remarks>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKey">The <see cref="PartitionKey"/> value if different than the <see cref="IItem.Id"/>.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous exists operation.</returns>
        ValueTask<bool> ExistsAsync(string id, PartitionKey partitionKey,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Queries cosmos DB to see if an item exists.
        /// </summary>
        /// <remarks>This checks the count of the resulting query any count greater than 1 will return true.</remarks>
        /// <param name="predicate">The expression used for evaluating any matching items.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous exists operation.</returns>
        ValueTask<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Queries cosmos DB to obtain the count of items.
        /// </summary>
        /// <remarks>
        /// This queries the total number of documents in the container.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous count operation.</returns>
        ValueTask<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Queries cosmos DB to obtain the count of items.
        /// </summary>
        /// <remarks>
        /// This queries the total number of documents in the container as filtered by the provided predicate.
        /// </remarks>
        /// <param name="predicate">The expression used for evaluating any matching items.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous count operation.</returns>
        ValueTask<int> CountAsync(
            Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Offers a load more paging implementation for infinite scroll scenarios.
        /// Allows for efficient paging making use of cosmos DBs continuation tokens, making this implementation cost effective.
        /// </summary>
        /// <param name="predicate">A filter criteria for the paging operation, if null it will get all <see cref="IItem"/>s</param>
        /// <param name="pageSize">The size of the page to return from cosmos db.</param>
        /// <param name="continuationToken">The token returned from a previous query, if null starts at the beginning of the data</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="IPage{T}"/> of <see cref="IItem"/>s</returns>
        /// <remarks>This method makes use of cosmos dbs continuation tokens for efficient, cost effective paging utilising low RUs</remarks>
        ValueTask<IPage<TItem>> PageAsync(
            Expression<Func<TItem, bool>> predicate = null,
            int pageSize = 25,
            string continuationToken = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Offers a load more paging implementation for infinite scroll scenarios.
        /// Allows for efficient paging making use of cosmos DBs continuation tokens, making this implementation cost effective.
        /// </summary>
        /// <param name="predicate">A filter criteria for the paging operation, if null it will get all <see cref="IItem"/>s</param>
        /// <param name="pageNumber">The page number to return from cosmos db.</param>
        /// <param name="pageSize">The size of the page to return from cosmos db.</param>
        /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
        /// <returns>A <see cref="IPageQueryResult{T}"/> of <see cref="IItem"/>s</returns>
        /// <remarks>This method makes use of Cosmos DB's continuation tokens for efficient, cost effective paging utilizing low RUs</remarks>

        ValueTask<IPageQueryResult<TItem>> PageAsync(
            Expression<Func<TItem, bool>> predicate = null,
            int pageNumber = 1,
            int pageSize = 25,
            CancellationToken cancellationToken = default);
    }
}
