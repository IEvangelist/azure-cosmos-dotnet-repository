// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

// ReSharper disable once CheckNamespace

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// This is the batch enabled repository interface for any implementation of
/// <typeparamref name="TItem"/>, exposing asynchronous batch update and create functionality.
/// </summary>
/// <typeparam name="TItem">The <see cref="IItem"/> implementation class type.</typeparam>
/// <example>
/// With DI, use .ctor injection to require any implementation of <see cref="IItem"/>:
/// <code language="c#">
/// <![CDATA[
/// public class ConsumingService
/// {
///     readonly IBatchRepository<SomePoco> _pocoRepository;
///
///     public ConsumingService(
///         IBatchRepository<SomePoco> pocoRepository) =>
///         _pocoRepository = pocoRepository;
/// }
/// ]]>
/// </code>
/// </example>
public interface IBatchRepository<in TItem>
    where TItem : IItem
{
    /// <summary>
    /// Updates an <see cref="IEnumerable{TItem}"/> as a batch.
    /// </summary>
    /// <param name="items">The items to update.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="BatchOperationException{TItem}">Thrown when the batch operation fails</exception>
    /// <returns>An <see cref="ValueTask"/> that represents the async batch operation.</returns>
    ValueTask UpdateAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an <see cref="IEnumerable{TItem}"/> as a batch.
    /// </summary>
    /// <param name="items">The items to create.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="BatchOperationException{TItem}">Thrown when the batch operation fails</exception>
    /// <returns>An <see cref="ValueTask"/> that represents the async batch operation.</returns>
    ValueTask CreateAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an <see cref="IEnumerable{TItem}"/> as a batch.
    /// </summary>
    /// <param name="items">The items to create.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="BatchOperationException{TItem}">Thrown when the batch operation fails</exception>
    /// <returns>An <see cref="ValueTask"/> that represents the async batch operation.</returns>
    ValueTask DeleteAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default);
}