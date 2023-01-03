// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed;

/// <summary>
/// A processor to process changes for the given <see cref="IItem"/>
/// </summary>
/// <typeparam name="TItem">The <see cref="IItem"/> the processor should track changes for.</typeparam>
public interface IItemChangeFeedProcessor<in TItem> where TItem : IItem
{
    /// <summary>
    /// Handles the changes for the given <see cref="IItem"/>.
    /// </summary>
    /// <param name="item">The <see cref="IItem"/> that has changed.</param>
    /// <param name="cancellationToken">The cancellation token to use when making asynchronous operations.</param>
    /// <returns>The <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask HandleAsync(TItem item, CancellationToken cancellationToken);
}