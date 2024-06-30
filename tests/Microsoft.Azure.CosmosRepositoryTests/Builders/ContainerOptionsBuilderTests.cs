// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Builders;

public class ExampleItem : Item
{
    [Required]
    [JsonProperty("testProperty")]
    public string TestProperty { get; set; } = null!;
}

public class ContainerOptionsBuilderTests
{
    [Fact]
    public void WithoutStrictTypeCheckingCorrectlyFlowsThroughBuilder()
    {
        // Arrange
        var builder = new ContainerOptionsBuilder(typeof(ExampleItem));

        // Act
        builder.WithoutStrictTypeChecking()
            .WithContainer("Example")
            .WithPartitionKey("/id");

        // Assert
        Assert.False(builder.UseStrictTypeChecking);
    }
}
