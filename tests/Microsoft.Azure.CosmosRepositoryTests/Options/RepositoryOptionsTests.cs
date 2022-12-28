// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Options;

public class RepositoryOptionsTests
{
    [Fact]
    public void RepositoryOptionsBuilderConfiguresItemCorrectly()
    {
        RepositoryOptions options = new();
        options.ContainerBuilder.Configure<Product>(
            containerOptions => containerOptions
                .WithContainer("products")
                .WithPartitionKey("/category")
        );

        Assert.Equal(1, options.ContainerOptions.Count);
        Assert.Equal("products", options.ContainerOptions[0].Name);
        Assert.Equal("/category", options.ContainerOptions[0].PartitionKey);
    }

    [Fact]
    public void RepositoryOptionsBuilderThrowsArgumentNullExceptionWhenContainerNameIsNull()
        => Assert.Throws<ArgumentNullException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(options => options.WithContainer(null!)));

    [Fact]
    public void RepositoryOptionsBuilderThrowsArgumentNullExceptionWhenPartitionKeyIsNull()
        => Assert.Throws<ArgumentNullException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(options => options.WithPartitionKey(null!)));

    [Fact]
    public void RepositoryOptionsBuilderThrowsArgumentNullExceptionWhenContainerBuilderActionIsNull()
        => Assert.Throws<ArgumentNullException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(null!));

    [Theory]
    [InlineData(300)]
    [InlineData(1_100_000)]
    public void RepositoryOptionsBuilderThrowsOutOfRangeExceptionWhenAutoscaleThroughputIsOutOfRange(int throughput) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(builder => builder.WithAutoscaleThroughput(throughput)));

    [Theory]
    [InlineData(1500)]
    [InlineData(1_150_000)]
    public void RepositoryOptionsBuilderThrowsInvalidOperationExceptionWhenAutoscaleThroughputNotAMultipleOf1000(int throughput) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(builder => builder.WithAutoscaleThroughput(throughput)));


    [Theory]
    [InlineData(200)]
    [InlineData(100)]
    public void RepositoryOptionsBuilderThrowsOutOfRangeExceptionWhenManualThroughputIsOutOfRange(int throughput) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(builder => builder.WithManualThroughput(throughput)));
}

public class Product : Item
{

}