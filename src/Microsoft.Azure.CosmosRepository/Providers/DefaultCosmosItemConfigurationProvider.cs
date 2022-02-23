// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    class DefaultCosmosItemConfigurationProvider : ICosmosItemConfigurationProvider
    {
        private static readonly ConcurrentDictionary<Type, ItemConfiguration> _itemOptionsMap = new();

        private readonly ICosmosContainerNameProvider _containerNameProvider;
        private readonly ICosmosPartitionKeyPathProvider _cosmosPartitionKeyPathProvider;
        private readonly ICosmosUniqueKeyPolicyProvider _cosmosUniqueKeyPolicyProvider;
        private readonly ICosmosContainerDefaultTimeToLiveProvider _containerDefaultTimeToLiveProvider;
        private readonly ICosmosContainerSyncContainerPropertiesProvider _syncContainerPropertiesProvider;
        private readonly ICosmosThroughputProvider _cosmosThroughputProvider;
        private readonly ICosmosStrictTypeCheckingProvider _cosmosStrictTypeCheckingProvider;

        public DefaultCosmosItemConfigurationProvider(
            ICosmosContainerNameProvider containerNameProvider,
            ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider,
            ICosmosUniqueKeyPolicyProvider cosmosUniqueKeyPolicyProvider,
            ICosmosContainerDefaultTimeToLiveProvider containerDefaultTimeToLiveProvider,
            ICosmosContainerSyncContainerPropertiesProvider syncContainerPropertiesProvider,
            ICosmosThroughputProvider cosmosThroughputProvider,
            ICosmosStrictTypeCheckingProvider cosmosStrictTypeCheckingProvider)
        {
            _containerNameProvider = containerNameProvider;
            _cosmosPartitionKeyPathProvider = cosmosPartitionKeyPathProvider;
            _cosmosUniqueKeyPolicyProvider = cosmosUniqueKeyPolicyProvider;
            _containerDefaultTimeToLiveProvider = containerDefaultTimeToLiveProvider;
            _syncContainerPropertiesProvider = syncContainerPropertiesProvider;
            _cosmosThroughputProvider = cosmosThroughputProvider;
            _cosmosStrictTypeCheckingProvider = cosmosStrictTypeCheckingProvider;
        }

        public ItemConfiguration GetItemConfiguration<TItem>() where TItem : IItem =>
            GetItemConfiguration(typeof(TItem));

        public ItemConfiguration GetItemConfiguration(Type itemType) =>
            _itemOptionsMap.GetOrAdd(itemType, AddOptions(itemType));


        private ItemConfiguration AddOptions(Type itemType)
        {
            itemType.IsItem();

            string containerName = _containerNameProvider.GetContainerName(itemType);
            string partitionKeyPath = _cosmosPartitionKeyPathProvider.GetPartitionKeyPath(itemType);
            UniqueKeyPolicy? uniqueKeyPolicy = _cosmosUniqueKeyPolicyProvider.GetUniqueKeyPolicy(itemType);
            int timeToLive = _containerDefaultTimeToLiveProvider.GetDefaultTimeToLive(itemType);
            bool sync = _syncContainerPropertiesProvider.GetWhetherToSyncContainerProperties(itemType);
            ThroughputProperties? throughputProperties = _cosmosThroughputProvider.GetThroughputProperties(itemType);
            bool useStrictTypeChecking = _cosmosStrictTypeCheckingProvider.UseStrictTypeChecking(itemType);

            return new(
                itemType,
                containerName,
                partitionKeyPath,
                uniqueKeyPolicy,
                throughputProperties,
                timeToLive,
                sync,
                useStrictTypeChecking: useStrictTypeChecking);
        }
    }
}