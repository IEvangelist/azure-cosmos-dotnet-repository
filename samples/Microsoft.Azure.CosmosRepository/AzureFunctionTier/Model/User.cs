using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace AzureFunctionTier.Model;

public class User : Item
{
    [JsonProperty("nickName")]
    public string? Nickname { get; set; }

    [JsonProperty("firstName")]
    public required string FirstName { get; set; }

    [JsonProperty("lastName")]
    public required string LastName { get; set; }

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    [JsonProperty("emailAddress")]
    public required string EmailAddress { get; set; }

    protected override string GetPartitionKeyValue()
    {
        return EmailAddress;
    }
}