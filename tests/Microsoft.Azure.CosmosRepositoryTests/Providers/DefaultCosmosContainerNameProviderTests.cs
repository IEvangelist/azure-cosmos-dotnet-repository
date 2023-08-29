// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultCosmosContainerNameProviderTests
{
    private readonly Mock<IOptions<RepositoryOptions>> _options;
    private readonly RepositoryOptions _repositoryOptions;

    public DefaultCosmosContainerNameProviderTests()
    {
        _options = new();
        _repositoryOptions = new();
        _options.SetupGet(o => o.Value).Returns(_repositoryOptions);
    }

    [Fact]
    public void CosmosContainerNameProviderGetsNameFromAttribute()
    {
        ICosmosContainerNameProvider provider = new DefaultCosmosContainerNameProvider(_options.Object);

        var name = provider.GetContainerName<CustomContainerNameItem>();
        Assert.Equal("SomethingCustom", name);
    }

    [Fact]
    public void CosmosContainerNameProviderGetsNameFromType()
    {
        ICosmosContainerNameProvider provider = new DefaultCosmosContainerNameProvider(_options.Object);

        var name = provider.GetContainerName<SomethingItem>();
        Assert.Equal("SomethingItem", name);
    }

    [Fact]
    public void CosmosContainerNameProviderGetsNameForTypeWhenProvidedByOptions()
    {
        _repositoryOptions.ContainerBuilder.Configure<CustomTypeOverridenByOptions>(options => options.WithContainer("SomethingDefinedByOptions"));

        ICosmosContainerNameProvider provider = new DefaultCosmosContainerNameProvider(_options.Object);

        var name = provider.GetContainerName<CustomTypeOverridenByOptions>();
        Assert.Equal("SomethingDefinedByOptions", name);
    }

    [Fact]
    public void CosmosContainerNameProviderGetsNameForTypeWhenEmptyStringProvidedByOptions()
    {
        _repositoryOptions.ContainerBuilder.Configure<SomeOtherItem>(options => options.WithContainer(""));

        ICosmosContainerNameProvider provider = new DefaultCosmosContainerNameProvider(_options.Object);

        var name = provider.GetContainerName<SomeOtherItem>();
        Assert.Equal("SomeOtherItem", name);
    }
}

[Container("SomethingCustom")]
public class CustomContainerNameItem : Item
{
}

[Container("SomethingCustom")]
public class CustomTypeOverridenByOptions : Item
{
}

public class SomethingItem : Item
{
}

public class SomeOtherItem : Item
{

}
