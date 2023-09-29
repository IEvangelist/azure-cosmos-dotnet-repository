// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

class DefaultCosmosItemConfigurationProvider(
    ICosmosContainerNameProvider containerNameProvider,
    ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider,
    ICosmosUniqueKeyPolicyProvider cosmosUniqueKeyPolicyProvider,
    ICosmosContainerDefaultTimeToLiveProvider containerDefaultTimeToLiveProvider,
    ICosmosContainerSyncContainerPropertiesProvider syncContainerPropertiesProvider,
    ICosmosThroughputProvider cosmosThroughputProvider,
    ICosmosStrictTypeCheckingProvider cosmosStrictTypeCheckingProvider) : ICosmosItemConfigurationProvider
{
    private static readonly ConcurrentDictionary<Type, ItemConfiguration> _itemOptionsMap = new();

    public ItemConfiguration GetItemConfiguration<TItem>() where TItem : IItem =>
        GetItemConfiguration(typeof(TItem));

    public ItemConfiguration GetItemConfiguration(Type itemType) =>
        _itemOptionsMap.GetOrAdd(itemType, AddOptions(itemType));


    private ItemConfiguration AddOptions(Type itemType)
    {
        itemType.IsItem();

        var containerName = containerNameProvider.GetContainerName(itemType);
        var partitionKeyPath = cosmosPartitionKeyPathProvider.GetPartitionKeyPath(itemType);
        UniqueKeyPolicy? uniqueKeyPolicy = cosmosUniqueKeyPolicyProvider.GetUniqueKeyPolicy(itemType);
        var timeToLive = containerDefaultTimeToLiveProvider.GetDefaultTimeToLive(itemType);
        var sync = syncContainerPropertiesProvider.GetWhetherToSyncContainerProperties(itemType);
        ThroughputProperties? throughputProperties = cosmosThroughputProvider.GetThroughputProperties(itemType);
        var useStrictTypeChecking = cosmosStrictTypeCheckingProvider.UseStrictTypeChecking(itemType);

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