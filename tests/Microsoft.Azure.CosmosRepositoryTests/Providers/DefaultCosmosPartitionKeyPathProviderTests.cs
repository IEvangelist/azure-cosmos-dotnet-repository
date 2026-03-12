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

        var path = provider.GetPartitionKeyPath<Person>();
        Assert.Equal("/firstName", path);
    }

    [Fact]
    public void CosmosCosmosPartitionKeyPathProviderCorrectlyGetsPathWhenOptionsAreDefinedButNull()
    {
        _repositoryOptions.ContainerBuilder.Configure<Person>(options => options.WithPartitionKey(""));

        ICosmosPartitionKeyPathProvider provider = new DefaultCosmosPartitionKeyPathProvider(_options.Object);

        var path = provider.GetPartitionKeyPath<AnotherPerson>();
        Assert.Equal("/email", path);
    }

    [Fact]
    public void CosmosPartitionKeyPathProviderCorrectlyGetsPathWhenAttributeIsDefined()
    {
        ICosmosPartitionKeyPathProvider provider = new DefaultCosmosPartitionKeyPathProvider(_options.Object);

        var path = provider.GetPartitionKeyPath<PickleChipsItem>();
        Assert.Equal("/pickles", path);
        Assert.Equal("[\"Hey, where's the chips?!\"]", new Cosmos.PartitionKey(((IItem)new PickleChipsItem()).PartitionKey).ToString());
    }

    [Fact]
    public void CosmosPartitionKeyPathProviderItemsThatShareAContainerWithEqualValuesReturnPath()
    {
        _repositoryOptions.ContainerBuilder.Configure<Person>(options =>
            options.WithContainer("people").WithPartitionKey("/email"));

        _repositoryOptions.ContainerBuilder.Configure<AnotherPerson>(options =>
            options.WithContainer("people"));

        ICosmosPartitionKeyPathProvider provider = new DefaultCosmosPartitionKeyPathProvider(_options.Object);

        string path = provider.GetPartitionKeyPath<Person>();

        Assert.Equal("/email", path);
    }

    [Fact]
    public void CosmosPartitionKeyPathProviderItemsThatShareAContainerWithConflictingValuesThrow()
    {
        _repositoryOptions.ContainerBuilder.Configure<Person>(options =>
            options.WithContainer("people").WithPartitionKey("/firstName"));

        _repositoryOptions.ContainerBuilder.Configure<AnotherPerson>(options =>
            options.WithContainer("people"));

        ICosmosPartitionKeyPathProvider provider = new DefaultCosmosPartitionKeyPathProvider(_options.Object);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => provider.GetPartitionKeyPath<Person>());

        Assert.Contains("partition key paths", exception.Message);
        Assert.Contains("/firstName", exception.Message);
        Assert.Contains("/email", exception.Message);
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
