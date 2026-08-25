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
    /// <remarks>
    /// Properties on nested objects are supported. The path is composed from the entire member access
    /// chain of the given <paramref name="expression"/>, where each segment is resolved using the
    /// <see cref="JsonPropertyAttribute"/> applied to the property when present, and the configured
    /// <see cref="CosmosPropertyNamingPolicy"/> otherwise. For example, <c>item => item.Address.City</c>
    /// operates on the <c>/address/city</c> path.
    /// </remarks>
    IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value);
}