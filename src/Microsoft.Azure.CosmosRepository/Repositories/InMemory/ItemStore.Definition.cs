// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.InMemory.Exceptions;
using Microsoft.Azure.CosmosRepository.InMemory.Reader;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed partial class ItemStore<TItem> : IItemStore<TItem> where TItem : IItem
    {
        private readonly IItemStoreWriterStrategy<TItem> _writerStrategy;
        private readonly IItemStoreReaderStrategy<TItem> _readerStrategy;
        private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
        private const string DefaultPartitionKey = "partitionKey";
        private readonly string _itemPartitionKey;

        public ItemStore(
            IItemStoreWriterStrategy<TItem> writerStrategy,
            IItemStoreReaderStrategy<TItem> readerStrategy,
            IOptionsMonitor<RepositoryOptions> optionsMonitor
            )
        {
            _writerStrategy = writerStrategy;
            _readerStrategy = readerStrategy;
            _optionsMonitor = optionsMonitor;
            _itemPartitionKey =
                _optionsMonitor.CurrentValue.GetContainerOptions<TItem>()?.PartitionKey?.Replace("/", string.Empty) ??
                DefaultPartitionKey;
        }

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, JObject>> _itemStore = new();
    }
}