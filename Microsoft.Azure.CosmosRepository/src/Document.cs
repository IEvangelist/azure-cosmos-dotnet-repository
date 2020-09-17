// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.CosmosRepository
{
    /// <summary>
    /// The base document used for all repository object or object graphs.
    /// </summary>
    /// <example>
    /// Here is an example subclass document, which adds several properties:
    /// <code language="c#">
    /// <![CDATA[
    /// public class SubDocument : Document
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
    public class Document
    {
        /// <summary>
        /// Gets or sets the document's globally unique identifier.
        /// </summary>
        /// <remarks>
        /// Initialized by <see cref="Guid.NewGuid"/>.
        /// </remarks>
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        internal PartitionKey PartitionKey => new PartitionKey(Id);
    }
}
