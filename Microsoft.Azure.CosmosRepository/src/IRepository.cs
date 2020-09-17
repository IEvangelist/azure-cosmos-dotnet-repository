// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository
{
    /// <summary>
    /// This is the repository interface for any implementation of 
    /// <typeparamref name="TDocument"/>, exposing asynchronous C.R.U.D. functionality.
    /// </summary>
    /// <typeparam name="TDocument">The <see cref="Document"/> subclass type.</typeparam>
    /// <example>
    /// With DI, use .ctor injection to require any subclass of <see cref="Document"/>:
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
    public interface IRepository<TDocument> where TDocument : Document
    {
        /// <summary>
        /// Gets the <see cref="Document"/> subclass instance as a <typeparamref name="TDocument"/> that corresponds to the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <typeparamref name="TDocument"/>.</returns>
        ValueTask<TDocument> GetAsync(string id);

        /// <summary>
        /// Gets an <see cref="IEnumerable{TDocument}"/> collection of <see cref="Document"/> 
        /// subclasses that match the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The expression used for evaluating a matching document.</param>
        /// <returns>A collection of document instances who meet the <paramref name="predicate"/> condition.</returns>
        ValueTask<IEnumerable<TDocument>> GetAsync(Expression<Func<TDocument, bool>> predicate);

        /// <summary>
        /// Creates a cosmos document representing the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The document value to create.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <typeparamref name="TDocument"/>.</returns>
        ValueTask<TDocument> CreateAsync(TDocument value);

        /// <summary>
        /// Creates one or more cosmos document(s) representing the given <paramref name="values"/>. 
        /// </summary>
        /// <param name="values">The document values to create.</param>
        /// <returns>A <see cref="Task{TDocument}"/></returns>
        Task<TDocument[]> CreateAsync(IEnumerable<TDocument> values);

        /// <summary>
        /// Updates the cosmos object that corresponds to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The document value to update.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <typeparamref name="TDocument"/>.</returns>
        ValueTask<TDocument> UpdateAsync(TDocument value);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The object to delete.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <typeparamref name="TDocument"/>.</returns>
        ValueTask<TDocument> DeleteAsync(TDocument value);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <typeparamref name="TDocument"/>.</returns>
        ValueTask<TDocument> DeleteAsync(string id);
    }
}