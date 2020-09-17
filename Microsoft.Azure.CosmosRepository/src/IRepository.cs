// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository
{
    /// <summary>
    /// This is the repository interface for any implementation of 
    /// <see cref="{T}"/>, exposing asynchronous C.R.U.D. functionality.
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
        /// Gets the <see cref="Document"/> subclass instance as a <see cref="{TDocument}"/> that corresponds to the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <see cref="{TDocument}"/>.</returns>
        ValueTask<TDocument> GetAsync(string id);

        /// <summary>
        /// Gets an <see cref="IEnumerable{TDocument}"/> collection of <see cref="Document"/> 
        /// subclasses that match the given <see cref="predicate"/>.
        /// </summary>
        /// <param name="predicate">The expression used for evaluating a matching document.</param>
        /// <returns>A <see cref="ValueTask{IEnumerable{TDocument}}"/> representing the <see cref="Document"/> subclass instances as a <see cref="{TDocument}"/>.</returns>
        ValueTask<IEnumerable<TDocument>> GetAsync(Expression<Func<TDocument, bool>> predicate);

        /// <summary>
        /// Creates a cosmos document representing the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The document value to create.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <see cref="{TDocument}"/>.</returns>
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
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <see cref="{TDocument}"/>.</returns>
        ValueTask<TDocument> UpdateAsync(TDocument value);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <see cref="{TDocument}"/>.</returns>
        ValueTask<TDocument> DeleteAsync(TDocument value);

        /// <summary>
        /// Deletes the cosmos object that corresponds to the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The string identifier.</param>
        /// <returns>A <see cref="ValueTask{TDocument}"/> representing the <see cref="Document"/> subclass instance as a <see cref="{TDocument}"/>.</returns>
        ValueTask<TDocument> DeleteAsync(string id);
    }
}