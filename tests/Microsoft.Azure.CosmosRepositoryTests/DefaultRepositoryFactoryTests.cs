// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


namespace Microsoft.Azure.CosmosRepositoryTests;

public class DefaultRepositoryFactoryTests
{
    [Fact]
    public void RepositoryFactoryCorrectlyGetsRepositoryUsingConnectionStringTest()
    {
        IConfigurationRoot configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RepositoryOptions:CosmosConnectionString"] = "Testing"
                })
                .Build();
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);

        services.AddCosmosRepository();

        IServiceProvider provider = services.BuildServiceProvider();
        IRepositoryFactory factory = provider.GetRequiredService<IRepositoryFactory>();

        Assert.NotNull(factory.RepositoryOf<AnotherTestItem>());
        Assert.NotNull(factory.RepositoryOf<AndAnotherItem>());
        Assert.NotNull(factory.RepositoryOf<AndACustomEntity>());
    }

    [Fact]
    public void RepositoryFactoryCorrectlyGetsRepositoryUsingTokenCredentialAuthenticationTest()
    {
        IConfigurationRoot configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RepositoryOptions:AccountEndpoint"] = "Account Endpoint"
                })
                .Build();
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);

        services.AddCosmosRepository(options => options.TokenCredential = new TestTokenCredential());

        IServiceProvider provider = services.BuildServiceProvider();
        IRepositoryFactory factory = provider.GetRequiredService<IRepositoryFactory>();

        Assert.NotNull(factory.RepositoryOf<AnotherTestItem>());
        Assert.NotNull(factory.RepositoryOf<AndAnotherItem>());
        Assert.NotNull(factory.RepositoryOf<AndACustomEntity>());
    }
}

public class AnotherTestItem : Item { }
public class AndAnotherItem : Item { }
public class AndACustomEntity : CustomEntityBase { }

/// <summary>
/// Sample custom base object that implements IItem
/// </summary>
public abstract class CustomEntityBase : IItem
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("quest")]
    public string Quest { get; set; } = null!;

    [JsonProperty("favoritecolor")]
    public string FavoriteColor { get; set; } = null!;

    string IItem.PartitionKey => GetPartitionKeyValue();

    IEnumerable<string> IItem.PartitionKeys => [GetPartitionKeyValue()];

    public CustomEntityBase() => Type = GetType().Name;

    protected virtual string GetPartitionKeyValue() => Id;
}
