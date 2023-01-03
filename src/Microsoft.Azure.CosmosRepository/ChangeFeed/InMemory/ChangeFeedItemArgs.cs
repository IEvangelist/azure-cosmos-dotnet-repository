// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory;

internal class ChangeFeedItemArgs<TItem> where TItem : IItem
{
    public IEnumerable<TItem> ItemChanges { get; private set; }

    public ChangeFeedItemArgs(IEnumerable<TItem> itemChanges) =>
        ItemChanges = itemChanges;

    public ChangeFeedItemArgs(TItem item) =>
        ItemChanges = new[] { item };
}