// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepositoryTests.Abstractions;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultContainerSyncContainerPropertiesProviderTests : WithRepositoryOptions
    {
        readonly DefaultContainerSyncContainerPropertiesProvider _provider;

        public DefaultContainerSyncContainerPropertiesProviderTests() => _provider = new DefaultContainerSyncContainerPropertiesProvider(_options.Object);

        [Fact]
        public void GetWhetherToSyncContainerPropertiesWhenSyncAllContainerPropertiesIsSetReturnsTrue()
        {
            _repositoryOptions.SyncAllContainerProperties = true;

            Assert.True(_provider.GetWhetherToSyncContainerProperties<TestItem>());
        }

        [Fact]
        public void GetWhetherToSyncContainerPropertiesWhenNoOptionsForItemReturnsFalse() =>
            Assert.False(_provider.GetWhetherToSyncContainerProperties<TestItem>());

        [Fact]
        public void GetWhetherToSyncContainerPropertiesWhenOptionsToSyncIsTrue()
        {
            _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder => builder.WithSyncableContainerProperties());
            Assert.True(_provider.GetWhetherToSyncContainerProperties<TestItem>());
        }

        [Fact]
        public void GetWhetherToSyncContainerPropertiesWhenOptionsToSyncIsFalse()
        {
            _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder => { });
            Assert.False(_provider.GetWhetherToSyncContainerProperties<TestItem>());
        }
    }
}