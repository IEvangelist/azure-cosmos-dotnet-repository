// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Aspire.Microsoft.Azure.CosmosRepository.Items;

/// <summary>
/// A base helper class that implements IItem
/// </summary>
/// <example>
/// Here is an example subclass item, which adds several properties:
/// <code language="c#">
/// <![CDATA[
/// public class SubItem : Item
/// {
///     public DateTimeOffset Date { get; set; }
///     public string Name { get; set; }
///     public IEnumerable<Child> Children { get; set; }
///     public IEnumerable<string> Tags { get; set; }
/// }
///
/// public class Child
/// {
///     public string Name { get; set; }
///     public DateTime BirthDate { get; set; }
/// }
/// ]]>
/// </code>
/// </example>
public abstract class Item : IItem
{
    /// <summary>
    /// Gets or sets the item's globally unique identifier.
    /// </summary>
    /// <remarks>
    /// Initialized by <see cref="Guid.NewGuid"/>.
    /// </remarks>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the item's type name. This is used as a discriminator.
    /// </summary>
    public string Type { get; set; } = null!;
}
