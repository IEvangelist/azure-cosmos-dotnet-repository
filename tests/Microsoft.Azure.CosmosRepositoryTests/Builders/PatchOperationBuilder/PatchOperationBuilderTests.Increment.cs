// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Builders
{
    public partial class PatchOperationBuilderTests
    {
        // Increment Tests
        [Fact]
        public void IncrementGivenPropertyIncreasesValueCorrectly()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Increment(x => x.TestIntProperty, 10);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/testIntProperty", operation.Path);
        }

        [Fact]
        public void IncrementDeeplyNestedPropertyUsingExpressionIncreasesValueCorrectly()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Increment(x => x.Address.ZipCode, 5);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/address/zipCode", operation.Path);
        }

        [Fact]
        public void IncrementDeeplyNestedPropertyUsingPathIncreasesValueCorrectly()
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

        [Fact]
        public void IncrementGivenPropertyWithNegativeValueDecreasesValueCorrectly()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Increment(x => x.TestIntProperty, -5);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/testIntProperty", operation.Path);
        }

        [Fact]
        public void IncrementGivenPropertyPathWithNegativeValueDecreasesValueCorrectly()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Increment("/address/zipCode", -10);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/address/zipCode", operation.Path);
        }

        [Fact]
        public void IncrementGivenPropertyWithDoubleValueIncreasesValueCorrectly()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Increment(x => x.TestIntProperty, 5.5);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/testIntProperty", operation.Path);
        }

        [Fact]
        public void IncrementGivenPropertyPathWithDoubleValueIncreasesValueCorrectly()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Increment("/address/zipCode", 3.5);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/address/zipCode", operation.Path);
        }

        [Fact]
        public void IncrementDeeplyNestedPropertyWithDoubleValueUsingExpressionIncreasesValueCorrectly()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Increment(x => x.Address.ZipCode, 2.5);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/address/zipCode", operation.Path);
        }

        [Fact]
        public void IncrementDeeplyNestedPropertyWithDoubleValueUsingPathIncreasesValueCorrectly()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Increment("/address/zipCode", 2.5);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/address/zipCode", operation.Path);
        }
    }
}
