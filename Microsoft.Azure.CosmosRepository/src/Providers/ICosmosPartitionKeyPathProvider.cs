// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <summary>
    /// The cosmos partition key path provider exposes the ability 
    /// to get an <see cref="Item"/>'s partition key path.
    /// </summary>
    interface ICosmosPartitionKeyPathProvider
    {
        /// <summary>
        /// Gets the partition key path for a given <typeparamref name="TItem"/> type.
        /// </summary>
        /// <typeparam name="TItem">The item for which the partition key path corresponds.</typeparam>
        /// <returns>A string value representing the partition key path, i.e.; "/partion"</returns>
        string GetPartitionKeyPath<TItem>() where TItem : Item;
    }
}
