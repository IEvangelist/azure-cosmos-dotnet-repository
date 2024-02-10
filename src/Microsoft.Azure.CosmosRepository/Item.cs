// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

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
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the item's type name. This is used as a discriminator.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets the PartitionKey based on <see cref="GetPartitionKeyValues"/> last record.
    /// Implemented explicitly to keep out of Item API
    /// </summary>
    string IItem.PartitionKey => GetPartitionKeyValues().Last();

    /// <summary>
    /// Gets the PartitionKeys based on <see cref="GetPartitionKeyValues"/>.
    /// Implemented explicitly to keep out of Item API
    /// </summary>
    IEnumerable<string> IItem.PartitionKeys => GetPartitionKeyValues();

    /// <summary>
    /// Default constructor, assigns type name to <see cref="Type"/> property.
    /// </summary>
    public Item() => Type = GetType().Name;

    /// <summary>
    /// Gets the partition key value for the given <see cref="Item"/> type.
    /// When overridden, be sure that the <see cref="PartitionKeyPathAttribute.Paths"/> values correspond
    /// to the <see cref="JsonPropertyAttribute.PropertyName"/> values, i.e.; "/partition" and "partition"
    /// respectively. If these two values do not correspond an error will occur.
    /// </summary>
    /// <returns>The <see cref="Item.Id"/> unless overridden by the subclass.</returns>
    protected virtual string GetPartitionKeyValue() => Id;

    /// <summary>
    /// Gets the partition key values for the given <see cref="Item"/> type.
    /// When overridden, be sure that the <see cref="PartitionKeyPathAttribute.Paths"/> values correspond
    /// to the <see cref="JsonPropertyAttribute.PropertyName"/> values, i.e.; "/partition" and "partition"
    /// respectively. If all provided key values do not have a matching property with the equivalent name, an error will occur.
    /// Make sure to add the latest inner 
    /// </summary>
    /// <returns>The list with <see cref="Item.Id"/> unless overridden by the subclass.</returns>
    protected virtual IEnumerable<string> GetPartitionKeyValues() => new string[] { GetPartitionKeyValue() };
}
