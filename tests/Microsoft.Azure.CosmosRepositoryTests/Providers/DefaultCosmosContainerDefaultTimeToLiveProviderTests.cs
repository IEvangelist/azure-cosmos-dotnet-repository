// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultCosmosContainerDefaultTimeToLiveProviderTests : WithRepositoryOptions
{
    readonly DefaultCosmosContainerDefaultTimeToLiveProvider _provider;

    public DefaultCosmosContainerDefaultTimeToLiveProviderTests() =>
        _provider = new DefaultCosmosContainerDefaultTimeToLiveProvider(_options.Object);

    [Fact]
    public void GetDefaultTimeToLiveItemWithNoOptionsReturnsMinus1() =>
        Assert.Equal(-1, _provider.GetDefaultTimeToLive<TestItemWithEtag>());

    [Fact]
    public void GetDefaultTimeToLiveItemWithOptionsUsesThatTimeToLive()
    {
        var timeToLive = TimeSpan.FromMinutes(5);
        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder =>
            builder.WithContainerDefaultTimeToLive(timeToLive));
    }

    [Fact]
    public void GetDefaultTimeToLiveItemWithOptionsThatShareAContainerNameWithEqualValues()
    {
        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder =>
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(5)).WithContainer("a"));

        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder =>
            builder.WithContainer("a"));

        Assert.Equal(5 * 60, _provider.GetDefaultTimeToLive<TestItemWithEtag>());
    }

    [Fact]
    public void GetDefaultTimeToLiveItemWithOptionsThatShareAContainerNameAndHaveConflictingValuesThrows()
    {
        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder =>
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(5)).WithContainer("a"));

        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder =>
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(6)).WithContainer("a"));

        Assert.Throws<InvalidOperationException>(() => _provider.GetDefaultTimeToLive<TestItemWithEtag>());
    }
}