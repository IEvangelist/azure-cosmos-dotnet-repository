using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace AzureFunctionTier.Model
{
    public class User : Item
    {
        [JsonProperty("nickName")]
        public string Nickname { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        protected override string GetPartitionKeyValue()
        {
            return EmailAddress;
        }
    }
}