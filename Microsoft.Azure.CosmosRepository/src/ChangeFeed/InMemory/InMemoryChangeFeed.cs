// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory
{
    /// <summary>
    /// Represents a change feed for a given <see cref="IItem"/>
    /// </summary>
    public class InMemoryChangeFeed<TItem> where TItem : class, IItem
    {
        private readonly IRepository<TItem> _repository;
        private readonly IItemChangeFeedProcessor<TItem> _changeFeedProcessor;

        internal InMemoryChangeFeed(IRepository<TItem> repository, IItemChangeFeedProcessor<TItem> changeFeedProcessor)
        {
            _repository = repository;
            _changeFeedProcessor = changeFeedProcessor;
        }

        /// <summary>
        /// Set's up the <see cref="InMemoryRepository{TItem}"/> to start listen to changes from the given <see cref="InMemoryRepository{TItem}"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="IRepository{TItem}"/> registered in DI is not a <see cref="InMemoryRepository{TItem}"/></exception>
        public void Setup()
        {
            if (_repository is InMemoryRepository<TItem> inMemoryRepository)
            {
                inMemoryRepository.Changes += (_, args) =>
                {
                    foreach (TItem changes in args.ItemChanges)
                    {
                        _changeFeedProcessor.HandleAsync(changes, default).AsTask().Wait();
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
}