// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// The cosmos partition key path provider exposes the ability
/// to get an <see cref="IItem"/>'s partition key path.
/// </summary>
interface ICosmosUniqueKeyPolicyProvider
{
    /// <summary>
    /// Gets the unique key policy for a given <typeparamref name="TItem"/> type.
    /// </summary>
    /// <typeparam name="TItem">The item for which the unique key policy corresponds.</typeparam>
    /// <returns>A <see cref="UniqueKeyPolicy"/> for the corresponding to the given <typeparamref name="TItem"/>.</returns>
    UniqueKeyPolicy? GetUniqueKeyPolicy<TItem>() where TItem : IItem;

    UniqueKeyPolicy? GetUniqueKeyPolicy(Type itemType);
}