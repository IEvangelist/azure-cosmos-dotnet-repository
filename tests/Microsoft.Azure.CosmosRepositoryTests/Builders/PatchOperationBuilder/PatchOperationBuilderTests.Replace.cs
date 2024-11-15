// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Builders
{
    public partial class PatchOperationBuilderTests
    {

        // Replace Tests
        [Fact]
        public void ReplaceGivenPropertyValueWithJsonAttributeSetsCorrectReplaceValue()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Replace(x => x.TestProperty, "100");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Replace, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void ReplaceGivenPropertyWithNoAttributesSetsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Replace(x => x.TestIntProperty, 50);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Replace, operation.OperationType);
            Assert.Equal("/testIntProperty", operation.Path);
        }

        [Fact]
        public void ReplaceGivenPropertyWithRequiredAttributeSetsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<RequiredItem> builder = new PatchOperationBuilder<RequiredItem>();

            // Act
            builder.Replace(x => x.TestProperty, "Test Value");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Replace, operation.OperationType);
            Assert.Equal("/testProperty", operation.Path);
        }

        [Fact]
        public void ReplaceGivenPropertyWithRequiredAndJsonAttributesSetsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<RequiredAndJsonItem> builder = new PatchOperationBuilder<RequiredAndJsonItem>();

            // Act
            builder.Replace(x => x.TestProperty, "Test Value");

            // Assert
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
        public void ReplaceDeeplyNestedPropertyListUsingExpressionSetsCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Replace("/address/tags/0", "United Kingdom");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Replace, operation.OperationType);
            Assert.Equal("/address/tags/0", operation.Path);
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
        public void ReplaceComplexNestedObjectUsingExpressionSetsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            var newCountry = new CountryInfo { Name = "Canada", CountryCode = "CA" };

            // Act
            builder.Replace(x => x.Address.Country, newCountry);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Replace, operation.OperationType);
            Assert.Equal("/address/country", operation.Path);
        }


        [Fact]
        public void ReplaceComplexNestedObjectUsingPathSetsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            var newCountry = new CountryInfo { Name = "Canada", CountryCode = "CA" };

            // Act
            builder.Replace("/address/country", newCountry);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Replace, operation.OperationType);
            Assert.Equal("/address/country", operation.Path);
        }

    }
}
