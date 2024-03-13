// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// This is the write-only repository interface for any implementation of
/// <typeparamref name="TItem"/>, exposing asynchronous create, update, and delete functionality.
/// </summary>
/// <typeparam name="TItem">The <see cref="IItem"/> implementation class type.</typeparam>
/// <example>
/// With DI, use .ctor injection to require any implementation of <see cref="IItem"/>:
/// <code language="c#">
/// <![CDATA[
/// public class ConsumingService
/// {
///     readonly IWriteOnlyRepository<SomePoco> _pocoRepository;
///
///     public ConsumingService(
///         IWriteOnlyRepository<SomePoco> pocoRepository) =>
///         _pocoRepository = pocoRepository;
/// }
/// ]]>
/// </code>
/// </example>
public interface IWriteOnlyRepository<TItem> where TItem : IItem
{
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
    /// <param name="ignoreEtag">When TItem implements IItemWithEtag the etag will be verified on all updates. Setting this flag to true indicates that the etag should be ignored.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>A <see cref="ValueTask{TItem}"/> representing the <see cref="IItem"/> implementation class instance as a <typeparamref name="TItem"/>.</returns>
    ValueTask<TItem> UpdateAsync(
        TItem value,
        bool ignoreEtag = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates one or more cosmos item(s) representing the given <paramref name="values"/>.
    /// </summary>
    /// <param name="values">The item values to update.</param>
    /// <param name="ignoreEtag">When TItem implements IItemWithEtag the etag will be verified on all updates. Setting this flag to true indicates that the etag should be ignored.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>A collection of updated item instances.</returns>
    ValueTask<IEnumerable<TItem>> UpdateAsync(
        IEnumerable<TItem> values,
        bool ignoreEtag = false,
        CancellationToken cancellationToken = default);

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
        string partitionKeyValue,
        string? etag = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the given cosmos item using the provided and supported patch operations.
    /// </summary>
    /// <param name="id">The string identifier.</param>
    /// <param name="builder">The <see cref="IPatchOperationBuilder{TItem}"/> that will define the update operations to perform.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <param name="etag">Indicate to set IfMatchEtag in the ItemRequestOptions in the underlying Cosmos call. This requires TItem to implement the IItemWithEtag interface.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask UpdateAsync(
        string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        string? etag = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the given cosmos item using the provided and supported patch operations.
    /// </summary>
    /// <param name="id">The string identifier.</param>
    /// <param name="partitionKeyValues">The partition key values if different than the <see cref="IItem.Id"/>.</param>
    /// <param name="builder">The <see cref="IPatchOperationBuilder{TItem}"/> that will define the update operations to perform.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <param name="etag">Indicate to set IfMatchEtag in the ItemRequestOptions in the underlying Cosmos call. This requires TItem to implement the IItemWithEtag interface.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask UpdateAsync(
        string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        IEnumerable<string> partitionKeyValues,
        string? etag = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the given cosmos item using the provided and supported patch operations.
    /// </summary>
    /// <param name="id">The string identifier.</param>
    /// <param name="partitionKey">The partition key if different than the <see cref="IItem.Id"/>.</param>
    /// <param name="builder">The <see cref="IPatchOperationBuilder{TItem}"/> that will define the update operations to perform.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <param name="etag">Indicate to set IfMatchEtag in the ItemRequestOptions in the underlying Cosmos call. This requires TItem to implement the IItemWithEtag interface.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask UpdateAsync(
        string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        PartitionKey partitionKey,
        string? etag = default,
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
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default);

    //TODO: Write docs
    ValueTask DeleteAsync(
        string id,
        IEnumerable<string> partitionKeyValues,
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
}
