// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory;

/// <summary>
/// Represents a change feed for a given <see cref="IItem"/>
/// </summary>
public class InMemoryChangeFeed<TItem> where TItem : class, IItem
{
    private readonly IRepository<TItem> _repository;
    private readonly IEnumerable<IItemChangeFeedProcessor<TItem>> _changeFeedProcessors;

    /// <summary>
    /// Creates an instance of an <see cref="InMemoryRepository{TItem}"/>
    /// </summary>
    /// <param name="repository">The instance of an <see cref="InMemoryRepository{TItem}"/> in which listen to changes</param>
    /// <param name="changeFeedProcessors">The processor that will be invoked when changes occur</param>
    public InMemoryChangeFeed(IRepository<TItem> repository,
        IEnumerable<IItemChangeFeedProcessor<TItem>> changeFeedProcessors)
    {
        _repository = repository;
        _changeFeedProcessors = changeFeedProcessors;
    }

    /// <summary>
    /// Set's up the <see cref="InMemoryRepository{TItem}"/> to start listen to changes from the given <see cref="InMemoryRepository{TItem}"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the <see cref="IRepository{TItem}"/> registered in DI is not a <see cref="InMemoryRepository{TItem}"/></exception>
    public void Setup()
    {
        if (_repository is InMemoryRepository<TItem> inMemoryRepository)
        {
            inMemoryRepository.Changes += (args) =>
            {
                foreach (TItem changes in args.ItemChanges)
                {
                    foreach (IItemChangeFeedProcessor<TItem> itemChangeFeedProcessor in _changeFeedProcessors)
                    {
                        itemChangeFeedProcessor.HandleAsync(changes, default).AsTask().Wait();
                    }
                }
            };
        }
        else
        {
            throw new InvalidOperationException(
                $"A repository of type {_repository.GetType().Name} cannot be setup to work with the InMemoryChangeFeed");
        }
    }
}