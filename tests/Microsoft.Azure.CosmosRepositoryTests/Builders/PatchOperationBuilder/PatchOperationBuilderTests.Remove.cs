// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Builders
{
    public partial class PatchOperationBuilderTests
    {
        // Remove Tests
        [Fact]
        public void RemoveGivenPropertyRemovesCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Remove(x => x.TestIntProperty);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Remove, operation.OperationType);
            Assert.Equal("/testIntProperty", operation.Path);
        }

        [Fact]
        public void RemoveDeeplyNestedPropertyListItemUsingPathRemovesCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Remove("/address/tags/0");

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
            Assert.Equal("/address/zipCode", operation.Path);
        }

        [Fact]
        public void RemoveNonNestedPropertyUsingPathRemovesCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Remove("/thisIsTheName");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Remove, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void RemoveDeeplyNestedPropertyWithJsonPropertyUsingExpressionRemovesCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Remove(x => x.Address.Country.CountryCode);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Remove, operation.OperationType);
            Assert.Equal("/address/country/countryCode", operation.Path);
        }

        [Fact]
        public void RemovePropertyWithJsonPropertyUsingPathRemovesCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<RequiredAndJsonItem> builder = new PatchOperationBuilder<RequiredAndJsonItem>();

            // Act
            builder.Remove("/testProperty");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Remove, operation.OperationType);
            Assert.Equal("/testProperty", operation.Path);
        }

        [Fact]
        public void RemoveComplexNestedPropertyUsingExpressionRemovesCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Remove(x => x.Address.Country);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Remove, operation.OperationType);
            Assert.Equal("/address/country", operation.Path);
        }

        [Fact]
        public void RemoveComplexNestedPropertyUsingPathRemovesCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Remove("/address/country");

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Remove, operation.OperationType);
            Assert.Equal("/address/country", operation.Path);
        }
    }
}


