// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    class DefaultCosmosItemConfigurationProvider : ICosmosItemConfigurationProvider
    {
        private static readonly ConcurrentDictionary<Type, ItemOptions> _itemOptionsMap = new();

        private readonly ICosmosContainerNameProvider _containerNameProvider;
        private readonly ICosmosPartitionKeyPathProvider _cosmosPartitionKeyPathProvider;
        private readonly ICosmosUniqueKeyPolicyProvider _cosmosUniqueKeyPolicyProvider;
        private readonly ICosmosContainerDefaultTimeToLiveProvider _containerDefaultTimeToLiveProvider;

        public DefaultCosmosItemConfigurationProvider(
            ICosmosContainerNameProvider containerNameProvider,
            ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider,
            ICosmosUniqueKeyPolicyProvider cosmosUniqueKeyPolicyProvider,
            ICosmosContainerDefaultTimeToLiveProvider containerDefaultTimeToLiveProvider)
        {
            _containerNameProvider = containerNameProvider;
            _cosmosPartitionKeyPathProvider = cosmosPartitionKeyPathProvider;
            _cosmosUniqueKeyPolicyProvider = cosmosUniqueKeyPolicyProvider;
            _containerDefaultTimeToLiveProvider = containerDefaultTimeToLiveProvider;
        }

        public ItemOptions GetOptions<TItem>() where TItem : IItem =>
            _itemOptionsMap.GetOrAdd(typeof(TItem), AddOptions<TItem>);

        private ItemOptions AddOptions<TItem>(Type optionType) where TItem : IItem
        {
            string containerName = _containerNameProvider.GetContainerName<TItem>();
            string partitionKeyPath = _cosmosPartitionKeyPathProvider.GetPartitionKeyPath<TItem>();
            UniqueKeyPolicy uniqueKeyPolicy = _cosmosUniqueKeyPolicyProvider.GetUniqueKeyPolicy<TItem>();
            int timeToLive = _containerDefaultTimeToLiveProvider.GetDefaultTimeToLive<TItem>();

            return new(optionType, containerName, partitionKeyPath, uniqueKeyPolicy, timeToLive);
        }
    }
}