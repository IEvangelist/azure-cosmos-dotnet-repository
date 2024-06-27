// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultCosmosThroughputProviderTests : WithRepositoryOptions
{
    readonly DefaultCosmosThroughputProvider _provider;

    public DefaultCosmosThroughputProviderTests() =>
        _provider = new DefaultCosmosThroughputProvider(_options.Object);

    [Fact]
    public void GetThroughputPropertiesGivenItemWithNoSettingsReturnManualThroughputPropertiesSetTo400RUs()
    {
        ThroughputProperties? throughputProperties = _provider.GetThroughputProperties<TestItemWithEtag>();

        Assert.Equal(400, throughputProperties!.Throughput);
        Assert.Null(throughputProperties.AutoscaleMaxThroughput);
    }

    [Fact]
    public void GetThroughputPropertiesItemAutoscaleThroughputProperties()
    {
        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder => builder.WithAutoscaleThroughput());
        ThroughputProperties? throughputProperties = _provider.GetThroughputProperties<TestItemWithEtag>();

        Assert.Equal(1000, throughputProperties!.AutoscaleMaxThroughput);
        Assert.Null(throughputProperties.Throughput);
    }

    [Fact]
    public void GetThroughputPropertiesItemManualThroughputProperties()
    {
        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder => builder.WithManualThroughput(500));
        ThroughputProperties? throughputProperties = _provider.GetThroughputProperties<TestItemWithEtag>();

        Assert.Equal(500, throughputProperties!.Throughput);
        Assert.Null(throughputProperties.AutoscaleMaxThroughput);
    }

    [Fact]
    public void GetThroughputPropertiesItemsWithConflictingAutoScaleValues()
    {
        _repositoryOptions.ContainerBuilder
            .Configure<TestItemWithEtag>(builder => builder.WithAutoscaleThroughput())
            .Configure<AnotherTestItem>(builder => builder.WithAutoscaleThroughput(5000));

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _provider.GetThroughputProperties<TestItemWithEtag>());

        Assert.Contains("autoscale", exception.Message);
        Assert.Contains("1000", exception.Message);
        Assert.Contains("5000", exception.Message);
    }

    [Fact]
    public void GetThroughputPropertiesItemsWithConflictingManualValues()
    {
        _repositoryOptions.ContainerBuilder
            .Configure<TestItemWithEtag>(builder => builder.WithManualThroughput(500))
            .Configure<AnotherTestItem>(builder => builder.WithManualThroughput(700));

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _provider.GetThroughputProperties<TestItemWithEtag>());

        Assert.Contains("manual", exception.Message);
        Assert.Contains("500", exception.Message);
        Assert.Contains("700", exception.Message);
    }

    [Fact]
    public void GetThroughputPropertiesContainerWithServerlessConfiguration()
    {
        _repositoryOptions.ContainerBuilder.Configure<AnotherTestItem>(
            builder => builder.WithServerlessThroughput());

        ThroughputProperties? throughputProperties = _provider.GetThroughputProperties<AnotherTestItem>();

        Assert.Null(throughputProperties);
    }
}