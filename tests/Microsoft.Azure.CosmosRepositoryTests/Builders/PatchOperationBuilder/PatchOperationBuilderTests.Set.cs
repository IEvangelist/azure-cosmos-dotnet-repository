// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Builders
{
    public partial class PatchOperationBuilderTests
    {
        // Set Tests
        [Fact]
        public void SetGivenPropertySetsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Set(x => x.TestIntProperty, 1);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/testIntProperty", operation.Path);
        }

        [Fact]
        public void SetNestedPropertyUsingExpressionSetsCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Set(x => x.Address.Street, "789 Oak St");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/address/street", operation.Path);
        }

        [Fact]
        public void SetNestedPropertyUsingPathSetsCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Set("/address/street", "789 Oak St");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/address/street", operation.Path);
        }

        [Fact]
        public void SetDeeplyNestedPropertyUsingExpressionSetsCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Set(x => x.Address.Country.Name, "Canada");

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
            builder.Set("/address/country/name", "Canada");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/address/country/name", operation.Path);
        }

        [Fact]
        public void SetListItemUsingPathSetsCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Set("/address/tags/1", "Updated Tag");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/address/tags/1", operation.Path);
        }

        [Fact]
        public void SetComplexNestedObjectUsingExpressionSetsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            var newCountry = new CountryInfo { Name = "Mexico", CountryCode = "MX" };

            // Act
            builder.Set(x => x.Address.Country, newCountry);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/address/country", operation.Path);
        }

        [Fact]
        public void SetComplexNestedObjectUsingPathSetsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            var newCountry = new CountryInfo { Name = "Mexico", CountryCode = "MX" };

            // Act
            builder.Set("/address/country", newCountry);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/address/country", operation.Path);
        }



    }
}