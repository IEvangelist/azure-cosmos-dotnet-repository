// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

/// <summary>
/// Allows a collection of <see cref="PatchOperation"/>'s to built./>
/// </summary>
public interface IPatchOperationBuilder<TItem> where TItem : IItem
{
    /// <summary>
    /// The currently built <see cref="PatchOperation"/>'s
    /// </summary>
    IReadOnlyList<PatchOperation> PatchOperations { get; }

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be replaced with the value provided
    /// </summary>
    /// <param name="expression">The expression to define which property to operate on.</param>
    /// <param name="value">The value to replace the property defined with.</param>
    /// <typeparam name="TValue">The type of the property that is been replaced.</typeparam>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be replaced with the value provided
    /// </summary>
    /// <param name="path">The path to replace</param>
    /// <param name="value">The value to replace the property defined with.</param>
    /// <typeparam name="TValue">The type of the property that is been replaced.</typeparam>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Replace<TValue>(string path, TValue value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be set with the value provided
    /// </summary>
    /// <typeparam name="TValue">The type of the property that is being set.</typeparam>
    /// <param name="expression">The expression to define which property to operate on.</param>
    /// <param name="value">The value to set the property defined with.</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Set<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be set with the value provided
    /// </summary>
    /// <typeparam name="TValue">The type of value to set.</typeparam>
    /// <param name="path">The json document path</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Set<TValue>(string path, TValue? value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be added with the value provided
    /// </summary>
    /// <typeparam name="TValue">The type of the property that is being added.</typeparam>
    /// <param name="expression">The expression to define which property to operate on.</param>
    /// <param name="value">The value to add to the document</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Add<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be added with the value provided
    /// </summary>
    /// <typeparam name="TValue">The type of the property that is being added.</typeparam>
    /// <param name="path">Target location reference.</param>
    /// <param name="value">The value to add to the document</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Add<TValue>(string path, TValue? value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be removed
    /// </summary>
    /// <typeparam name="TValue">The type of the property that is being removed.</typeparam>
    /// <param name="expression">The expression to define which property to operate on.</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Remove<TValue>(Expression<Func<TItem, TValue>> expression);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be removed
    /// </summary>
    /// <param name="path">Target location reference.</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Remove(string path);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be incremented with the value provided.
    /// </summary>
    /// <param name="path">Target location reference.</param>
    /// <param name="value">The value to add to the document.</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Increment(string path, long value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be incremented with the value provided
    /// </summary>
    /// <typeparam name="TValue">The type of the property that is meant to be incremented.</typeparam>
    /// <param name="expression">The expression to define which property to operate on.</param>
    /// <param name="value">The value to add to the document</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, double value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be incremented with the value provided
    /// </summary>
    /// <param name="path">Target location reference.</param>
    /// <param name="value">The value to increment.</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Increment(string path, double value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be incremented with the value provided
    /// </summary>
    /// <typeparam name="TValue">The type of the property that is meant to be incremented.</typeparam>
    /// <param name="expression">The expression to define which property to operate on.</param>
    /// <param name="value">The value to increment</param>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, long value);
}