// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository
{
    /// <summary>
    /// The base interface used for all repository object or object graphs.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Gets or sets the item's globally unique identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the item's type name.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets the item's PartitionKey.
        /// </summary>
        string PartitionKey { get; }
    }
}
