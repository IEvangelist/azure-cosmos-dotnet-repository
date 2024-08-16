// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

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

        Assert.Single(options.ContainerOptions);
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
    [InlineData(90)]
    [InlineData(1_100_000)]
    public void RepositoryOptionsBuilderThrowsOutOfRangeExceptionWhenAutoscaleThroughputIsOutOfRange(int throughput) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(builder => builder.WithAutoscaleThroughput(throughput)));

    [Theory]
    [InlineData(1500)]
    [InlineData(2200)]
    public void RepositoryOptionsBuilderThrowsInvalidOperationExceptionWhenAutoscaleThroughputNotAMultipleOf1000(int throughput) =>
        Assert.Throws<InvalidOperationException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(builder => builder.WithAutoscaleThroughput(throughput)));

    [Theory]
    [InlineData(200)]
    [InlineData(100)]
    public void RepositoryOptionsBuilderThrowsOutOfRangeExceptionWhenManualThroughputIsOutOfRange(int throughput) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(builder => builder.WithManualThroughput(throughput)));
}

public class Product : Item
{

}