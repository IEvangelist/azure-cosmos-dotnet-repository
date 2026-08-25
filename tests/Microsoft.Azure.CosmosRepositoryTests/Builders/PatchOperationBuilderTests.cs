// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Builders;

public class Item1 : Item
{
    [JsonProperty("thisIsTheName")]
    public string TestProperty { get; set; } = null!;

    public int TestIntProperty { get; set; }
}

public class RequiredItem : Item
{
    [Required]
    public string TestProperty { get; set; } = null!;
}

public class RequiredAndJsonItem : Item
{
    [Required]
    [JsonProperty("testProperty")]
    public string TestProperty { get; set; } = null!;
}

public class NestedItem : Item
{
    public NestedItemAddress Address { get; set; } = null!;
}

public class NestedItemAddress
{
    public string City { get; set; } = null!;

    [JsonProperty("zip_code")]
    public string PostCode { get; set; } = null!;
}

public class PatchOperationBuilderTests
{
    [Fact]
    public void ReplaceGivenPropertyValueWithJsonAttributeSetsCorrectReplaceValue()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Replace(x => x.TestProperty, "100");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/thisIsTheName", operation.Path);
    }

    [Fact]
    public void ReplaceGivenPropertyWithNoAttributesSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Replace(x => x.TestIntProperty, 50);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void ReplaceGivenPropertyWithRequiredAttributeSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<RequiredItem> builder = new PatchOperationBuilder<RequiredItem>();

        //Act
        builder.Replace(x => x.TestProperty, "Test Value");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/testProperty", operation.Path);
    }

    [Fact]
    public void ReplaceGivenPropertyWithRequiredAndJsonAttributesSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<RequiredAndJsonItem> builder = new PatchOperationBuilder<RequiredAndJsonItem>();

        //Act
        builder.Replace(x => x.TestProperty, "Test Value");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/testProperty", operation.Path);
    }

    [Fact]
    public void ReplaceGivenNestedPropertySetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<NestedItem> builder = new PatchOperationBuilder<NestedItem>();

        //Act
        builder.Replace(x => x.Address.City, "Amsterdam");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/address/city", operation.Path);
    }

    [Fact]
    public void ReplaceGivenNestedPropertyWithJsonAttributeSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<NestedItem> builder = new PatchOperationBuilder<NestedItem>();

        //Act
        builder.Replace(x => x.Address.PostCode, "1011AB");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/address/zip_code", operation.Path);
    }

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void AcknowledgeRepositorySerializationSettingForRetrievingPatchOperation(CosmosPropertyNamingPolicy? propertyNamingPolicy,
        string expectedPropertyName)
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>(propertyNamingPolicy);

        //Act
        builder.Replace(x => x.TestIntProperty, 1234);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal($"/{expectedPropertyName}", operation.Path);
    }

    public static IEnumerable<object?[]> GetTestCases()
    {
        yield return new object?[]
        {
            CosmosPropertyNamingPolicy.CamelCase,
            new CamelCaseNamingStrategy().GetPropertyName(nameof(Item1.TestIntProperty), false)
        };
        yield return new object?[]
        {
            CosmosPropertyNamingPolicy.Default,
            new DefaultNamingStrategy().GetPropertyName(nameof(Item1.TestIntProperty), false)
        };
        yield return new object?[]
        {
            null,
            new CamelCaseNamingStrategy().GetPropertyName(nameof(Item1.TestIntProperty), false)
        };
    }
}