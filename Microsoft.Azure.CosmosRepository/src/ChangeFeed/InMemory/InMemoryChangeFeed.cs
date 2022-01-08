// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory
{
    /// <summary>
    ///
    /// </summary>
    public class InMemoryChangeFeed<TItem> where TItem : class, IItem
    {
        private readonly IRepository<TItem> _repository;
        private readonly IItemChangeFeedProcessor<TItem> _changeFeedProcessor;

        /// <summary>
        ///
        /// </summary>
        public InMemoryChangeFeed(IRepository<TItem> repository, IItemChangeFeedProcessor<TItem> changeFeedProcessor)
        {
            _repository = repository;
            _changeFeedProcessor = changeFeedProcessor;
        }

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
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