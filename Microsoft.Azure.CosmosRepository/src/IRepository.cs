﻿// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// This is the repository interface for any implementation of <typeparamref name="TItem" />,
    /// exposing asynchronous C.R.U.D. functionality.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Item" /> subclass type.</typeparam>
    /// <example>
    /// With DI, use .ctor injection to require any subclass of <see cref="Item" />:
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
    public interface IRepository<TItem> where TItem : Item
    {
        /// <summary>
        /// Creates a cosmos item representing the given <paramref name="value" />.
        /// </summary>
        /// <param name="value">The item value to create.</param>
        /// <returns>
        /// A <see cref="ValueTask{TItem}" /> representing the <see cref="Item" /> subclass instance
        /// as a <typeparamref name="TItem" />.
        /// </returns>
        ValueTask<TItem> CreateAsync(TItem value);

        /// <summary>
        /// Creates one or more cosmos item(s) representing the given <paramref name="values" />.
        /// </summary>
        /// <param name="values">The item values to create.</param>
        /// <returns>A collection of created item instances.</returns>
        ValueTask<IEnumerable<TItem>> CreateAsync(IEnumerable<TItem> values);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="value" />.
        /// </summary>
        /// <param name="value">The object to delete.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous delete operation.</returns>
        ValueTask DeleteAsync(TItem value);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="id" />.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKeyValue">
        /// The partition key value if different than the <see cref="Item.Id" />.
        /// </param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous delete operation.</returns>
        ValueTask DeleteAsync(string id, string? partitionKeyValue = null);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="id" />.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKey">
        /// The <see cref="PartitionKey" /> value if different than the <see cref="Item.Id" />.
        /// </param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous delete operation.</returns>
        ValueTask DeleteAsync(string id, PartitionKey partitionKey);

        /// <summary>
        /// Enumerates an <see cref="IEnumerable{TItem}" /> collection of <see cref="Item" />
        /// subclasses that match the given <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The expression used for evaluating a matching item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A collection of item instances who meet the <paramref name="predicate" /> condition.
        /// </returns>
        /// <remarks>
        /// If the typeof( <typeparamref name="TItem" />).Name differs from the item.Type you're
        /// attempting to retrieve, the item is not returned.
        /// </remarks>
        IAsyncEnumerable<TItem> EnumerateAsync(
            Expression<Func<TItem, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="Item" /> subclass instance as a <typeparamref name="TItem" /> that
        /// corresponds to the given <paramref name="id" />.
        /// </summary>
        /// <remarks>
        /// If the typeof( <typeparamref name="TItem" />).Name differs from the item.Type you're
        /// attempting to retrieve, null is returned.
        /// </remarks>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKeyValue">
        /// The partition key value if different than the <see cref="Item.Id" />.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask{TItem}" /> representing the <see cref="Item" /> subclass instance
        /// as a <typeparamref name="TItem" />.
        /// </returns>
        ValueTask<TItem?> GetAsync(string id, string? partitionKeyValue = null);

        /// <summary>
        /// Gets the <see cref="Item" /> subclass instance as a <typeparamref name="TItem" /> that
        /// corresponds to the given <paramref name="id" />.
        /// </summary>
        /// <remarks>
        /// If the typeof( <typeparamref name="TItem" />).Name differs from the item.Type you're
        /// attempting to retrieve, null is returned.
        /// </remarks>
        /// <param name="id">The string identifier.</param>
        /// <param name="partitionKey">
        /// The <see cref="PartitionKey" /> value if different than the <see cref="Item.Id" />.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask{TItem}" /> representing the <see cref="Item" /> subclass instance
        /// as a <typeparamref name="TItem" />.
        /// </returns>
        ValueTask<TItem?> GetAsync(string id, PartitionKey partitionKey);

        /// <summary>
        /// Gets an <see cref="IEnumerable{TItem}" /> collection of <see cref="Item" /> subclasses
        /// that match the given <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The expression used for evaluating a matching item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A collection of item instances who meet the <paramref name="predicate" /> condition.
        /// </returns>
        /// <remarks>
        /// If the typeof( <typeparamref name="TItem" />).Name differs from the item.Type you're
        /// attempting to retrieve, the item is not returned.
        /// </remarks>
        ValueTask<IEnumerable<TItem>> GetAsync(
            Expression<Func<TItem, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the cosmos object that corresponds to the given <paramref name="value" />.
        /// </summary>
        /// <param name="value">The item value to update.</param>
        /// <returns>
        /// A <see cref="ValueTask{TItem}" /> representing the <see cref="Item" /> subclass instance
        /// as a <typeparamref name="TItem" />.
        /// </returns>
        ValueTask<TItem> UpdateAsync(TItem value);
    }
}
