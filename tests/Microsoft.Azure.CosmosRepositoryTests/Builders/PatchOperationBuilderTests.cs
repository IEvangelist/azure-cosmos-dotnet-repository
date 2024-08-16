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

public class Address
{
    public string Street { get; set; } = null!;
    public int ZipCode { get; set; }

    public CountryInfo Country { get; set; } = new CountryInfo();

    public string[] Tags { get; set; } = default!;
}

public class CountryInfo
{
    public string Name { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
}

public class Person : Item
{
    public string Name { get; set; } = null!;
    public Address Address { get; set; } = null!;
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
    public void ReplaceDeeplyNestedPropertyUsingExpressionReplacesCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Replace(x => x.Address.Country.CountryCode, "UK");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/address/country/countryCode", operation.Path);
    }

    [Fact]
    public void ReplaceDeeplyNestedPropertyUsingPathReplacesCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Replace("/address/country/countryCode", "UK");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/address/country/countryCode", operation.Path);
    }

    [Fact]
    public void SetGivenPropertySetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Set(x => x.TestIntProperty, 100);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Set, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void SetDeeplyNestedPropertyUsingExpressionSetsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Set(x => x.Address.Country.Name, "United Kingdom");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Set, operation.OperationType);
        Assert.Equal("/address/country/name", operation.Path);
    }

    [Fact]
    public void SetDeeplyNestedPropertyUsingPathSetsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Set("/address/country/name", "United Kingdom");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Set, operation.OperationType);
        Assert.Equal("/address/country/name", operation.Path);
    }

    [Fact]
    public void AddGivenPropertyAddsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Add(x => x.TestIntProperty, 200);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Add, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void AddDeeplyNestedPropertyUsingExpressionAddsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Add(x => x.Address.Tags, ["NewTag"]);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Add, operation.OperationType);
        Assert.Equal("/address/tags/0", operation.Path);
    }

    [Fact]
    public void AddDeeplyNestedPropertyUsingPathAddsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Add("/address/tags/-", "NewTag");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Add, operation.OperationType);
        Assert.Equal("/address/tags/0", operation.Path);
    }

    [Fact]
    public void RemoveGivenPropertyRemovesCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Remove(x => x.TestIntProperty);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Remove, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void IncrementGivenPropertyWithDoubleIncrementsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Increment(x => x.TestIntProperty, 1.5);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Increment, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void IncrementGivenPropertyWithLongIncrementsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Increment(x => x.TestIntProperty, 10);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Increment, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void ReplaceGivenPathSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Replace("/testIntProperty", 50);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void SetGivenPathSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Set("/testIntProperty", 100);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Set, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void AddGivenPathSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Add("/testIntProperty", 200);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Add, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void RemoveGivenPathSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Remove("/testIntProperty");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Remove, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void RemoveDeeplyNestedPropertyUsingExpressionRemovesCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Remove(x => x.Address.Tags[0]);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Remove, operation.OperationType);
        Assert.Equal("/address/tags/0", operation.Path);
    }

    [Fact]
    public void RemoveDeeplyNestedPropertyUsingPathRemovesCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Remove("/address/tags");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Remove, operation.OperationType);
        Assert.Equal("/address/tags", operation.Path);
    }

    [Fact]
    public void IncrementGivenPathWithDoubleIncrementsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Increment("/testIntProperty", 1.5);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Increment, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void IncrementGivenPathWithLongIncrementsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Increment("/testIntProperty", 10);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Increment, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void ReplaceNestedPropertyUsingExpressionSetsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Replace(x => x.Address.Street, "123 Main St");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/address/street", operation.Path);
    }
    [Fact]
    public void ReplaceDeeplyNestedPropertyUsingExpressionSetsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Replace(x => x.Address.Country.Name, "United Kingdom");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/address/country/name", operation.Path);
    }

    [Fact]
    public void ReplaceNestedPropertyUsingPathSetsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Replace("/address/street", "456 Elm St");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/address/street", operation.Path);
    }

    [Fact]
    public void SetNestedPropertyUsingExpressionSetsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Set(x => x.Address.ZipCode, 90210);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Set, operation.OperationType);
        Assert.Equal("/address/zipCode", operation.Path);
    }

    [Fact]
    public void SetNestedPropertyUsingPathSetsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Set("/address/zipCode", 12345);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Set, operation.OperationType);
        Assert.Equal("/address/zipCode", operation.Path);
    }

    [Fact]
    public void AddNestedPropertyUsingExpressionAddsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Add(x => x.Address.ZipCode, 11111);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Add, operation.OperationType);
        Assert.Equal("/address/zipCode", operation.Path);
    }

    [Fact]
    public void AddNestedPropertyUsingPathAddsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Add("/address/zipCode", 22222);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Add, operation.OperationType);
        Assert.Equal("/address/zipCode", operation.Path);
    }

    [Fact]
    public void RemoveNestedPropertyUsingExpressionRemovesCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Remove(x => x.Address.ZipCode);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Remove, operation.OperationType);
        Assert.Equal("/address/zipCode", operation.Path);
    }

    [Fact]
    public void RemoveNestedPropertyUsingPathRemovesCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Remove("/address/zipCode");

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Remove, operation.OperationType);
        Assert.Equal("/address/zipCode", operation.Path); // Assuming camelCase naming strategy
    }

    [Fact]
    public void IncrementNestedPropertyUsingExpressionIncrementsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Increment(x => x.Address.ZipCode, 10);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Increment, operation.OperationType);
        Assert.Equal("/address/zipCode", operation.Path);
    }

    [Fact]
    public void IncrementNestedPropertyUsingPathIncrementsCorrectValue()
    {
        // Arrange
        IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

        // Act
        builder.Increment("/address/zipCode", 5);

        // Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Increment, operation.OperationType);
        Assert.Equal("/address/zipCode", operation.Path);
    }


    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void AcknowledgeRepositorySerializationSettingForRetrievingPatchOperation(CosmosPropertyNamingPolicy? propertyNamingPolicy,
        string expectedPropertyName)
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>(propertyNamingPolicy);

        //Act
        builder.Add(x => x.TestIntProperty, 1);
        builder.Set(x => x.TestIntProperty, 2);
        builder.Replace(x => x.TestIntProperty, 3);
        builder.Increment(x => x.TestIntProperty, 1);
        builder.Remove(x => x.TestIntProperty);

        //Assert
        var paths = builder.PatchOperations.Select(x => x.Path);

        // Check that all paths are equal to the expected value
        Assert.All(paths, path => Assert.Equal($"/{expectedPropertyName}", path));
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