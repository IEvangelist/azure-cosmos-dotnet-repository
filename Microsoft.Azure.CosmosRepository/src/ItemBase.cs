// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
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
    /// public class SubItem : ItemBase
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
    public abstract class ItemBase
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
        /// Gets or sets the item's type name.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        internal PartitionKey PartitionKey => SetPartitionKey();

        /// <summary>
        /// Allows for setting a <see cref="PartitionKey"/> other than <see cref="Id"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual PartitionKey SetPartitionKey()
        {
            return new PartitionKey(Id);
        }

        /// <summary>
        /// Default constructor, assigns type name to <see cref="Type"/> property.
        /// </summary>
        protected ItemBase() => Type = GetType().Name;
    }
}
