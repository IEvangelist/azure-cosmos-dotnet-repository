// Copyright © IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Attributes;

using Newtonsoft.Json;

using System;

namespace Microsoft.Azure.CosmosRepository
{
	/// <summary>
	/// The base item used for all repository object or object graphs.
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
	public abstract class Item
	{
		/// <summary>
		/// Gets or sets the item's globally unique identifier.
		/// </summary>
		/// <value>The item's globally unique identifier.</value>
		/// <remarks>
		/// Initialized by <see cref="Guid.NewGuid"/>.
		/// </remarks>
		[JsonProperty("id")]
		public string Id { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		/// Gets or sets the item's type name.
		/// </summary>
		/// <value>The item's type name.</value>
		[JsonProperty("type")]
		public string Type { get; set; }

		internal PartitionKey PartitionKey => new PartitionKey(GetPartitionKeyValue());

		/// <summary>
		/// Initializes a new instance of the <see cref="Item"/> class.
		/// </summary>
		public Item() => Type = GetType().Name;

		/// <summary>
		/// Gets the partition key value for the given <see cref="Item"/> type.
		/// When overridden, be sure that the <see cref="PartitionKeyPathAttribute.Path"/> value corresponds
		/// to the <see cref="JsonPropertyAttribute.PropertyName"/> value, i.e.; "/partition" and "partition"
		/// respectively. If these two values do not correspond an error will occur.
		/// </summary>
		/// <returns>The <see cref="Id"/> unless overridden by the subclass.</returns>
		protected virtual string GetPartitionKeyValue() => Id;
	}
}
