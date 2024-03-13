// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultCosmosItemConfigurationProviderTests
{
    readonly Mock<ICosmosContainerNameProvider> _containerNameProvider = new();
    readonly Mock<ICosmosPartitionKeyPathProvider> _partitionKeyPathProvider = new();
    readonly Mock<ICosmosUniqueKeyPolicyProvider> _uniqueKeyPolicyProvider = new();
    readonly Mock<ICosmosContainerDefaultTimeToLiveProvider> _defaultTimeToLiveProvider = new();
    readonly Mock<ICosmosContainerSyncContainerPropertiesProvider> _syncContainerPropertiesProvider = new();
    readonly Mock<ICosmosThroughputProvider> _throughputProvider = new();
    readonly Mock<ICosmosStrictTypeCheckingProvider> _strictTypeCheckingProvider = new();

    [Fact]
    public void GetOptionsAlwaysGetOptionsForItem()
    {
        ICosmosItemConfigurationProvider provider = new DefaultCosmosItemConfigurationProvider(
            _containerNameProvider.Object,
            _partitionKeyPathProvider.Object,
            _uniqueKeyPolicyProvider.Object,
            _defaultTimeToLiveProvider.Object,
            _syncContainerPropertiesProvider.Object,
            _throughputProvider.Object,
            _strictTypeCheckingProvider.Object);

        UniqueKeyPolicy uniqueKeyPolicy = new();
        var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(400);

        _containerNameProvider.Setup(o => o.GetContainerName(typeof(Item1))).Returns<Type>(t => t.FullName!);
        _partitionKeyPathProvider.Setup(o => o.GetPartitionKeyPaths(typeof(Item1))).Returns(["/id"]);
        _uniqueKeyPolicyProvider.Setup(o => o.GetUniqueKeyPolicy(typeof(Item1))).Returns(uniqueKeyPolicy);
        _defaultTimeToLiveProvider.Setup(o => o.GetDefaultTimeToLive(typeof(Item1))).Returns(10);
        _syncContainerPropertiesProvider.Setup(o => o.GetWhetherToSyncContainerProperties(typeof(Item1))).Returns(true);
        _throughputProvider.Setup(o => o.GetThroughputProperties(typeof(Item1))).Returns(throughputProperties);

        ItemConfiguration configuration = provider.GetItemConfiguration<Item1>();

        Assert.Equal(typeof(Item1).FullName, configuration.ContainerName);
        Assert.Equal("/id", configuration.PartitionKeyPaths.Last());
        Assert.Equal(uniqueKeyPolicy, configuration.UniqueKeyPolicy);
        Assert.Equal(10, configuration.DefaultTimeToLive);
        Assert.True(configuration.SyncContainerProperties);
        Assert.Equal(throughputProperties, configuration.ThroughputProperties);
    }

    [Fact]
    public void GetAllItemConfigurationsAlwaysGetsAllOptionsForItems()
    {
        ICosmosItemConfigurationProvider provider = new DefaultCosmosItemConfigurationProvider(
            _containerNameProvider.Object,
            _partitionKeyPathProvider.Object,
            _uniqueKeyPolicyProvider.Object,
            _defaultTimeToLiveProvider.Object,
            _syncContainerPropertiesProvider.Object,
            _throughputProvider.Object,
            _strictTypeCheckingProvider.Object);

        _containerNameProvider.Setup(o => o.GetContainerName(It.IsAny<Type>())).Returns<Type>(t => t.FullName!);

        IEnumerable<string> expectedContainerNames = new[] {typeof(Item1).Assembly}
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IItem).IsAssignableFrom(p) && p is {IsInterface: false, IsAbstract: false})
            .Select(t => t.FullName!).OrderBy(name => name);

        IEnumerable<string> containerNames = provider.GetAllItemConfigurations(typeof(Item1).Assembly).Select(c => c.ContainerName).OrderBy(name => name);

        containerNames.Should().BeEquivalentTo(expectedContainerNames);
    }

    class Item1 : Item
    {

    }

    class Item2 : Item
    {

    }
}