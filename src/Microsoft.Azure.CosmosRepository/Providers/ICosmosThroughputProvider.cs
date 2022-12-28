// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// Exposes the ability to get an <see cref="IItem"/>s containers throughput properties.
/// </summary>
interface ICosmosThroughputProvider
{
    /// <summary>
    /// Gets the throughput properties for the given <see cref="IItem"/>s container.
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="IItem"/></typeparam>
    /// <returns><see cref="ThroughputProperties"/> for the <see cref="IItem"/>s container.</returns>
    ThroughputProperties? GetThroughputProperties<TItem>() where TItem : IItem;

    ThroughputProperties? GetThroughputProperties(Type itemType);
}