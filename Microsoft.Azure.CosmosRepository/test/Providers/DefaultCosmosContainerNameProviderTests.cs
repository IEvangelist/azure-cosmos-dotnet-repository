// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository.Providers;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosContainerNameProviderTests
    {
        [Fact]
        public void CosmosContainerNameProviderGetsNameFromAttribute()
        {
            ICosmosContainerNameProvider provider = new DefaultCosmosContainerNameProvider();

            string name = provider.GetContainerName<CustomContainerNameItem>();
            Assert.Equal("SomethingCustom", name);
        }

        [Fact]
        public void CosmosContainerNameProviderGetsNameFromType()
        {
            ICosmosContainerNameProvider provider = new DefaultCosmosContainerNameProvider();

            string name = provider.GetContainerName<SomethingItem>();
            Assert.Equal("SomethingItem", name);
        }
    }

    [Container("SomethingCustom")]
    public class CustomContainerNameItem : Item
    {
    }

    public class SomethingItem : Item
    {
    }
}
