using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;

namespace ServiceTier
{
    [PartitionKeyPath("/partitionKey")]
    public class Place : ItemBase
    {
        public string Location { get; set; }
        public string Name { get; set; }
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        protected override PartitionKey SetPartitionKey()
        {
            return new PartitionKey(PartitionKey);
        }
    }
}