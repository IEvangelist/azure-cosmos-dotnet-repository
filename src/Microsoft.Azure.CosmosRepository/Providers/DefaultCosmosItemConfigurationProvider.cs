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

    public List<ItemConfiguration> GetAllItemConfigurations(params Assembly[]? assemblies)
    {
        IEnumerable<Type> itemTypes = (assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IItem).IsAssignableFrom(p) && p is {IsInterface: false, IsAbstract: false});

        foreach (Type itemType in itemTypes)
        {
            _itemOptionsMap.GetOrAdd(itemType, AddOptions(itemType));
        }

        return _itemOptionsMap.Select(i => i.Value).ToList();
    }

    private ItemConfiguration AddOptions(Type itemType)
    {
        itemType.IsItem();

        var containerName = containerNameProvider.GetContainerName(itemType);
        var partitionKeyPaths = cosmosPartitionKeyPathProvider.GetPartitionKeyPaths(itemType);
        UniqueKeyPolicy? uniqueKeyPolicy = cosmosUniqueKeyPolicyProvider.GetUniqueKeyPolicy(itemType);
        var timeToLive = containerDefaultTimeToLiveProvider.GetDefaultTimeToLive(itemType);
        var sync = syncContainerPropertiesProvider.GetWhetherToSyncContainerProperties(itemType);
        ThroughputProperties? throughputProperties = cosmosThroughputProvider.GetThroughputProperties(itemType);
        var useStrictTypeChecking = cosmosStrictTypeCheckingProvider.UseStrictTypeChecking(itemType);

        return new(
            itemType,
            containerName,
            partitionKeyPaths,
            uniqueKeyPolicy,
            throughputProperties,
            timeToLive,
            sync,
            useStrictTypeChecking: useStrictTypeChecking);

    }
}