// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultContainerSyncContainerPropertiesProviderTests : WithRepositoryOptions
{
    readonly DefaultContainerSyncContainerPropertiesProvider _provider;

    public DefaultContainerSyncContainerPropertiesProviderTests() => _provider = new DefaultContainerSyncContainerPropertiesProvider(_options.Object);

    [Fact]
    public void GetWhetherToSyncContainerPropertiesWhenSyncAllContainerPropertiesIsSetReturnsTrue()
    {
        _repositoryOptions.SyncAllContainerProperties = true;

        Assert.True(_provider.GetWhetherToSyncContainerProperties<TestItemWithEtag>());
    }

    [Fact]
    public void GetWhetherToSyncContainerPropertiesWhenNoOptionsForItemReturnsFalse() =>
        Assert.False(_provider.GetWhetherToSyncContainerProperties<TestItemWithEtag>());

    [Fact]
    public void GetWhetherToSyncContainerPropertiesWhenOptionsToSyncIsTrue()
    {
        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder => builder.WithSyncableContainerProperties());
        Assert.True(_provider.GetWhetherToSyncContainerProperties<TestItemWithEtag>());
    }

    [Fact]
    public void GetWhetherToSyncContainerPropertiesWhenOptionsToSyncIsFalse()
    {
        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder => { });
        Assert.False(_provider.GetWhetherToSyncContainerProperties<TestItemWithEtag>());
    }
}