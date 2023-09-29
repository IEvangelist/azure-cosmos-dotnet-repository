// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Attributes;

/// <summary>
/// The container attribute exposes the ability to declaratively
/// specify an <see cref="IItem"/>'s container name. This attribute can only be used when
/// <see cref="RepositoryOptions.ContainerPerItemType"/> is set to <c>true</c>.
/// </summary>
/// <remarks>
/// Constructor accepting the <paramref name="name"/> of the container for a given <see cref="IItem"/>.
/// </remarks>
/// <param name="name"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ContainerAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the path of the parition key.
    /// </summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name), "A name is required.");
}
