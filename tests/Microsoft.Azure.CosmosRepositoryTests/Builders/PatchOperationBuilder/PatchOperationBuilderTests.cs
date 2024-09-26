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

    public CountryInfo[] RelatedCountries { get; set; } = default!;
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

public partial class PatchOperationBuilderTests
{

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void AcknowledgeRepositorySerializationSettingForRetrievingPatchOperation(CosmosPropertyNamingPolicy? propertyNamingPolicy,
        string expectedPropertyName)
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>(propertyNamingPolicy);

        //Act
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
