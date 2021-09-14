// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <summary>
    /// Exposes the ability to get an <see cref="IItem"/>s containers throughput properties.
    /// </summary>
    public interface ICosmosThroughputProvider
    {
        /// <summary>
        /// Gets the throughput properties for the given <see cref="IItem"/>s container.
        /// </summary>
        /// <typeparam name="TItem">The type of <see cref="IItem"/></typeparam>
        /// <returns><see cref="ThroughputProperties"/> for the <see cref="IItem"/>s container.</returns>
        ThroughputProperties GetThroughputProperties<TItem>() where TItem : IItem;
    }
}