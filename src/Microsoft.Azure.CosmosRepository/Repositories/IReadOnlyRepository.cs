// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// This is the read-only repository interface for any implementation of
/// <typeparamref name="TItem"/>, exposing asynchronous read functionality.
/// </summary>
/// <typeparam name="TItem">The <see cref="IItem"/> implementation class type.</typeparam>
/// <example>
/// With DI, use .ctor injection to require any implementation of <see cref="IItem"/>:
/// <code language="c#">
/// <![CDATA[
/// public class ConsumingService
/// {
///     readonly IReadOnlyRepository<SomePoco> _pocoRepository;
///
///     public ConsumingService(
///         IReadOnlyRepository<SomePoco> pocoRepository) =>
///         _pocoRepository = pocoRepository;
/// }
/// ]]>
/// </code>
/// </example>
public interface IReadOnlyRepository<TItem> where TItem : IItem
{
    /// <summary>
    /// Attempts to get an <see cref="IItem"/> that corresponds to the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The string identifier.</param>
    /// <param name="partitionKeyValue">The partition key value if different than the <see cref="IItem.Id"/>.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <remarks>This implementation handles the case in which a <see cref="CosmosException"/> with the status code of 404.
    /// It will return null when this exception is thrown.</remarks>
    /// <returns></returns>
    ValueTask<TItem?> TryGetAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default);

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
        string? partitionKeyValue = null,
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
    /// Queries cosmos DB to see if an item exists.
    /// </summary>
    /// <remarks>This method performs a point read to decide whether or not an item exists.</remarks>
    /// <param name="id">The string identifier.</param>
    /// <param name="partitionKeyValue">The partition key value if different than the <see cref="IItem.Id"/>.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous exists operation.</returns>
    ValueTask<bool> ExistsAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries cosmos DB to see if an item exists.
    /// </summary>
    /// <remarks>This method performs a point read to decide whether or not an item exists.</remarks>
    /// <param name="id">The string identifier.</param>
    /// <param name="partitionKey">The <see cref="PartitionKey"/> value if different than the <see cref="IItem.Id"/>.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous exists operation.</returns>
    ValueTask<bool> ExistsAsync(
        string id,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries cosmos DB to see if an item exists.
    /// </summary>
    /// <remarks>This checks the count of the resulting query any count greater than 1 will return true.</remarks>
    /// <param name="predicate">The expression used for evaluating any matching items.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous exists operation.</returns>
    ValueTask<bool> ExistsAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries cosmos DB to obtain the count of items.
    /// </summary>
    /// <remarks>
    /// This queries the total number of documents in the container.
    /// </remarks>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous count operation.</returns>
    ValueTask<int> CountAsync(
        CancellationToken cancellationToken = default);

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
    /// <param name="returnTotal">Specifies whether or not to return the total number of items that matched the query. This defaults to false as it can be a very expensive operation.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>An <see cref="IPage{T}"/> of <see cref="IItem"/>s</returns>
    /// <remarks>This method makes use of cosmos dbs continuation tokens for efficient, cost effective paging utilising low RUs</remarks>
    ValueTask<IPage<TItem>> PageAsync(
        Expression<Func<TItem, bool>>? predicate = null,
        int pageSize = 25,
        string? continuationToken = null,
        bool returnTotal = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items based on a specification.
    /// The specification is used to define which filters are used, the order of the search results and how they are paged.
    /// Depending on how results are paged derive specification implementations from different classes:
    /// For non paged results derive <see cref="DefaultSpecification{TItem}"/>
    /// For continuation token derive <see cref="ContinuationTokenSpecification{T}"/>
    /// For page number results derive <see cref="OffsetByPageNumberSpecification{T}"/>
    /// </summary>
    /// <typeparam name="TResult">Decides which paging information is retrieved. Use <see cref="ContinuationTokenSpecification{T}"/></typeparam>
    /// <param name="specification">A specification used to filtering, ordering and paging. A <see cref="ISpecification{T, TResult}"/></param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>The selected <typeparamref name="TResult"/> implementation that implements <see cref="IQueryResult{T}"/> of <see cref="IItem"/></returns>
    /// <remarks>This method makes use of cosmos dbs continuation tokens for efficient, cost effective paging utilising low RUs</remarks>
    ValueTask<TResult> QueryAsync<TResult>(
        ISpecification<TItem, TResult> specification,
        CancellationToken cancellationToken = default)
        where TResult : IQueryResult<TItem>;

    /// <summary>
    /// Offers a load more paging implementation for infinite scroll scenarios.
    /// Allows for efficient paging making use of cosmos DBs continuation tokens, making this implementation cost effective.
    /// </summary>
    /// <param name="predicate">A filter criteria for the paging operation, if null it will get all <see cref="IItem"/>s</param>
    /// <param name="pageNumber">The page number to return from cosmos db.</param>
    /// <param name="pageSize">The size of the page to return from cosmos db.</param>
    /// <param name="returnTotal">Specifies whether or not to return the total number of items that matched the query. This defaults to false as it can be a very expensive operation.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>An <see cref="IPageQueryResult{T}"/> of <see cref="IItem"/>s</returns>
    /// <remarks>This method makes use of Cosmos DB's continuation tokens for efficient, cost effective paging utilizing low RUs</remarks>
    ValueTask<IPageQueryResult<TItem>> PageAsync(
        Expression<Func<TItem, bool>>? predicate = null,
        int pageNumber = 1,
        int pageSize = 25,
        bool returnTotal = false,
        CancellationToken cancellationToken = default);

#if NET7_0_OR_GREATER
    /// <summary>
    /// Wraps the existing paging support to return an <see cref="IAsyncEnumerable{T}"/>
    /// where <c>T</c> is <typeparamref name="TItem"/>.
    /// </summary>
    /// <param name="predicate">A filter criteria for the paging operation, if null it will get all <see cref="IItem"/>s</param>
    /// <param name="limit">The limit of how many items to yield. Defaults to <c>1,000</c>.</param>
    /// <param name="pageSize">The size of the page to return from cosmos db.</param>
    /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> used to </param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> where <c>T</c> is <typeparamref name="TItem"/>.</returns>
    /// <remarks>This method makes use of Cosmos DB's continuation tokens for efficient, cost effective paging utilizing low RUs</remarks>
    async IAsyncEnumerable<TItem> PageAsync(
        Expression<Func<TItem, bool>>? predicate = null,
        int limit = 1_000,
        int pageSize = 25,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var collected = 0;
        var currentPage = 0;
        var hasMoreResults = true;

        while (hasMoreResults && collected < limit
            && cancellationToken.IsCancellationRequested is false)
        {
            IPageQueryResult<TItem> page = await PageAsync(
                predicate,
                pageNumber: ++ currentPage,
                pageSize,
                returnTotal: false,
                cancellationToken);

            hasMoreResults = page.HasNextPage.GetValueOrDefault();

            foreach (TItem item in page.Items)
            {
                if (collected < limit)
                {
                    yield return item;
                }

                collected ++;
            }
        }
    }
#endif
}
