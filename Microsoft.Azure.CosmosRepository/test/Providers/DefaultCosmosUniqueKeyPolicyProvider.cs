// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository.Providers;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosUniqueKeyPolicyProviderTests
    {
        [Fact]
        public void CosmosUniqueKeyPolicyProviderCorrectlyGetsOneUniqueKey()
        {
            ICosmosUniqueKeyPolicyProvider provider = new DefaultCosmosUniqueKeyPolicyProvider();

            UniqueKeyPolicy policy  = provider.GetUniqueKeyPolicy<SomeInterestingClass>();
            Assert.Equal("/Name", policy.UniqueKeys.Single().Paths.Single());
            
        }

        [Fact]
        public void CosmosUniqueKeyPolicyProviderCorrectlyGetsUniqueKeysWithTwoPaths()
        {
            ICosmosUniqueKeyPolicyProvider provider = new DefaultCosmosUniqueKeyPolicyProvider();

            UniqueKeyPolicy policy = provider.GetUniqueKeyPolicy<SomeInterestingClass2>();
            Assert.Contains("/Street", policy.UniqueKeys.Single().Paths);
            Assert.Contains("/HouseNumber", policy.UniqueKeys.Single().Paths);
            Assert.Equal(2, policy.UniqueKeys.Single().Paths.Count);
        }

        [Fact]
        public void CosmosUniqueKeyPolicyProviderCorrectlyGetsTwoUniqueKeys()
        {
            ICosmosUniqueKeyPolicyProvider provider = new DefaultCosmosUniqueKeyPolicyProvider();

            UniqueKeyPolicy policy = provider.GetUniqueKeyPolicy<SomeInterestingClass3>();
            UniqueKey key1 = policy.UniqueKeys.Single((key) => key.Paths.Count == 2);
            UniqueKey key2 = policy.UniqueKeys.Single((key) => key.Paths.Count == 1);

            Assert.Contains("/Street", key1.Paths);
            Assert.Contains("/HouseNumber", key1.Paths);
            Assert.Equal(2, key1.Paths.Count);

            Assert.Contains("/Name", key2.Paths);
            Assert.Single(key2.Paths);
        }
    }

    public class SomeInterestingClass : Item
    {
        public string Street { get; set; } = "Street1";
        public string HouseNumber { get; set; } = "1";

        [UniqueKey("nameKey")]
        public string Name { get; set; } = "John";

        protected override string GetPartitionKeyValue() => Name;
    }

    public class SomeInterestingClass2 : Item
    {
        [UniqueKey("addressKey")]
        public string Street { get; set; } = "Street1";
        [UniqueKey("addressKey")]
        public string HouseNumber { get; set; } = "1";

        public string Name { get; set; } = "John";

        protected override string GetPartitionKeyValue() => Name;
    }

    public class SomeInterestingClass3 : Item
    {
        [UniqueKey("addressKey")]
        public string Street { get; set; } = "Street1";
        [UniqueKey("addressKey")]
        public string HouseNumber { get; set; } = "1";

        [UniqueKey("nameKey")]
        public string Name { get; set; } = "John";

        protected override string GetPartitionKeyValue() => Name;
    }
}
