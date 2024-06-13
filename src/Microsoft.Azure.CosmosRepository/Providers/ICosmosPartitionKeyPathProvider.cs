// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// The cosmos partition key path provider exposes the ability
/// to get an <see cref="IItem"/>'s partition key path.
/// </summary>
interface ICosmosPartitionKeyPathProvider
{
    /// <summary>
    /// Gets the partition key paths for a given <typeparamref name="TItem"/> type.
    /// </summary>
    /// <typeparam name="TItem">The item for which the partition keys paths corresponds.</typeparam>
    /// <returns>A string array representing the partition key paths, i.e.; "/partion"</returns>
    IEnumerable<string> GetPartitionKeyPaths<TItem>() where TItem : IItem;
    
    IEnumerable<string> GetPartitionKeyPaths(Type itemType);
}
