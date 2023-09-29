// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory;

/// <summary>
/// Represents a change feed for a given <see cref="IItem"/>
/// </summary>
/// <remarks>
/// Creates an instance of an <see cref="InMemoryRepository{TItem}"/>
/// </remarks>
/// <param name="repository">The instance of an <see cref="InMemoryRepository{TItem}"/> in which listen to changes</param>
/// <param name="changeFeedProcessors">The processor that will be invoked when changes occur</param>
public class InMemoryChangeFeed<TItem>(IRepository<TItem> repository,
    IEnumerable<IItemChangeFeedProcessor<TItem>> changeFeedProcessors) where TItem : class, IItem
{

    /// <summary>
    /// Set's up the <see cref="InMemoryRepository{TItem}"/> to start listen to changes from the given <see cref="InMemoryRepository{TItem}"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the <see cref="IRepository{TItem}"/> registered in DI is not a <see cref="InMemoryRepository{TItem}"/></exception>
    public void Setup()
    {
        if (repository is InMemoryRepository<TItem> inMemoryRepository)
        {
            inMemoryRepository.Changes += (args) =>
            {
                foreach (TItem changes in args.ItemChanges)
                {
                    foreach (IItemChangeFeedProcessor<TItem> itemChangeFeedProcessor in changeFeedProcessors)
                    {
                        itemChangeFeedProcessor.HandleAsync(changes, default).AsTask().Wait();
                    }
                }
            };
        }
        else
        {
            throw new InvalidOperationException(
                $"A repository of type {repository.GetType().Name} cannot be setup to work with the InMemoryChangeFeed");
        }
    }
}