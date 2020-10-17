// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    using Microsoft.Azure.CosmosRepository;
    using Microsoft.Azure.CosmosRepository.Attributes;
    using Microsoft.Azure.CosmosRepository.Providers;
    using Newtonsoft.Json;
    using Xunit;

    public class DefaultCosmosPartitionKeyPathProviderTests
    {
        [Fact]
        public void CosmosPartitionKeyPathProviderCorrectlyGetsPath()
        {
            ICosmosPartitionKeyPathProvider provider = new DefaultCosmosPartitionKeyPathProvider();

            string path = provider.GetPartitionKeyPath<PickleChipsItem>();
            Assert.Equal("/pickles", path);
            Assert.Equal("[\"Hey, where's the chips?!\"]", new PickleChipsItem().PartitionKey.ToString());
        }
    }

    [PartitionKeyPath("/pickles")]
    public class PickleChipsItem : Item
    {
        [JsonProperty(PropertyName = "pickles")]
        public string PartitionBits { get; set; } = "Hey, where's the chips?!";

        protected override string GetPartitionKeyValue() => this.PartitionBits;
    }
}
