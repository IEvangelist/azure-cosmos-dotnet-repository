// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Attributes;

/// <summary>
/// The unique key attribute exposes the ability to declaratively
/// specify an <see cref="IItem"/>'s properties that can contribute to a unique key constraint.
/// For more information, see https://docs.microsoft.com/azure/cosmos-db/unique-keys.
/// </summary>
/// <remarks>
/// Constructor accepting the <paramref name="keyName"/> for a given <see cref="IItem"/>'s property.
/// </remarks>
/// <param name="keyName"></param>
/// <param name="propertyPath">The property path to match for the constraint</param>
/// <remarks>If the propertyPath is null the name of the property this is defined will be used.</remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class UniqueKeyAttribute(string? keyName = null, string? propertyPath = null) : Attribute
{
    /// <summary>
    /// Gets the key name that represents the unique key.
    /// </summary>
    /// <remarks>This is the unique name to match a set of paths on</remarks>
    public string KeyName { get; } = keyName ?? "onlyUniqueKey";

    /// <summary>
    /// The property path to use for the key
    /// </summary>
    public string? PropertyPath { get; set; } = propertyPath;
}
