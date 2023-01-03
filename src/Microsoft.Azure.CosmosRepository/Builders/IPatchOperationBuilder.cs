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
    /// <remarks>This currently only supports operations on properties on the root level of a JSON document,
    /// replacing properties on a nested object for example are currently not supported.</remarks>
    IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value);
}