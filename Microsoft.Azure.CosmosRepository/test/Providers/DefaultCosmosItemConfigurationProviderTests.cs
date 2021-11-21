// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosItemConfigurationProviderTests
    {
        readonly Mock<ICosmosContainerNameProvider> _containerNameProvider = new();
        readonly Mock<ICosmosPartitionKeyPathProvider> _partitionKeyPathProvider = new();
        readonly Mock<ICosmosUniqueKeyPolicyProvider> _uniqueKeyPolicyProvider = new();
        readonly Mock<ICosmosContainerDefaultTimeToLiveProvider> _defaultTimeToLiveProvider = new();
        readonly Mock<ICosmosContainerSyncContainerPropertiesProvider> _syncContainerPropertiesProvider = new();
        readonly Mock<ICosmosThroughputProvider> _throughputProvider = new();

        [Fact]
        public void GetOptionsAlwaysGetOptionsForItem()
        {
            ICosmosItemConfigurationProvider provider = new DefaultCosmosItemConfigurationProvider(
                _containerNameProvider.Object,
                _partitionKeyPathProvider.Object,
                _uniqueKeyPolicyProvider.Object,
                _defaultTimeToLiveProvider.Object,
                _syncContainerPropertiesProvider.Object,
                _throughputProvider.Object);

            UniqueKeyPolicy uniqueKeyPolicy = new();
            ThroughputProperties throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(400);

            _containerNameProvider.Setup(o => o.GetContainerName<Item1>()).Returns("a");
            _partitionKeyPathProvider.Setup(o => o.GetPartitionKeyPath<Item1>()).Returns("/id");
            _uniqueKeyPolicyProvider.Setup(o => o.GetUniqueKeyPolicy<Item1>()).Returns(uniqueKeyPolicy);
            _defaultTimeToLiveProvider.Setup(o => o.GetDefaultTimeToLive<Item1>()).Returns(10);
            _syncContainerPropertiesProvider.Setup(o => o.GetWhetherToSyncContainerProperties<Item1>()).Returns(true);
            _throughputProvider.Setup(o => o.GetThroughputProperties<Item1>()).Returns(throughputProperties);

            ItemOptions options = provider.GetOptions<Item1>();

            Assert.Equal("a", options.ContainerName);
            Assert.Equal("/id", options.PartitionKeyPath);
            Assert.Equal(uniqueKeyPolicy, options.UniqueKeyPolicy);
            Assert.Equal(10, options.DefaultTimeToLive);
            Assert.True(options.SyncContainerProperties);
            Assert.Equal(throughputProperties, options.ThroughputProperties);
        }

        class Item1 : Item
        {

        }
    }
}