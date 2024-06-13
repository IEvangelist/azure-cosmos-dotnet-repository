// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultCosmosPartitionKeyPathProviderTests
{
    private readonly Mock<IOptions<RepositoryOptions>> _options;
    private readonly RepositoryOptions _repositoryOptions;

    public DefaultCosmosPartitionKeyPathProviderTests()
    {
        _options = new();
        _repositoryOptions = new();
        _options.SetupGet(o => o.Value).Returns(_repositoryOptions);
    }

    [Fact]
    public void CosmosCosmosPartitionKeyPathProviderCorrectlyGetsPathWhenOptionsAreDefined()
    {
        _repositoryOptions.ContainerBuilder.Configure<Person>(options => options.WithPartitionKey("/firstName"));

        ICosmosPartitionKeyPathProvider provider = new DefaultCosmosPartitionKeyPathProvider(_options.Object);

        var paths = provider.GetPartitionKeyPaths<Person>();

        Assert.NotEmpty(paths);
        Assert.Single(paths);
        Assert.Equal("/firstName", paths.First());
    }

    [Fact]
    public void CosmosCosmosPartitionKeyPathProviderCorrectlyGetsPathWhenOptionsAreDefinedButNull()
    {
        _repositoryOptions.ContainerBuilder.Configure<Person>(options => options.WithPartitionKey(""));

        ICosmosPartitionKeyPathProvider provider = new DefaultCosmosPartitionKeyPathProvider(_options.Object);

        var paths = provider.GetPartitionKeyPaths<AnotherPerson>();
        Assert.NotEmpty(paths);
        Assert.Single(paths);
        Assert.Equal("/email", paths.First());
    }

    [Fact]
    public void CosmosPartitionKeyPathProviderCorrectlyGetsPathWhenAttributeIsDefined()
    {
        ICosmosPartitionKeyPathProvider provider = new DefaultCosmosPartitionKeyPathProvider(_options.Object);

        var paths = provider.GetPartitionKeyPaths<PickleChipsItem>();

        Assert.NotEmpty(paths);
        Assert.Single(paths);
        Assert.Equal("/pickles", paths.First());
        Assert.Equal("[\"Hey, where's the chips?!\"]", new Cosmos.PartitionKey(((IItem)new PickleChipsItem()).PartitionKey).ToString());
    }
}

[PartitionKeyPath("/email")]
public class AnotherPerson : Item
{

}

[PartitionKeyPath("/email")]
public class Person : Item
{

}

[PartitionKeyPath("/pickles")]
public class PickleChipsItem : Item
{
    [JsonProperty(PropertyName = "pickles")]
    public string PartitionBits { get; set; } = "Hey, where's the chips?!";

    protected override string GetPartitionKeyValue() => PartitionBits;
}
