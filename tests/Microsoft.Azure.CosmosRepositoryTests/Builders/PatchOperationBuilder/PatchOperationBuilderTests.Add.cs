// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Builders
{
    public partial class PatchOperationBuilderTests
    {

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void AcknowledgeRepositorySerializationSettingForRetrievingAddPatchOperation(CosmosPropertyNamingPolicy? propertyNamingPolicy,
        string expectedPropertyName)
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>(propertyNamingPolicy);

            //Act
            builder.Add(x => x.TestIntProperty, 1);

            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal($"/{expectedPropertyName}/-", operation.Path);

        }

        // Add Tests
        [Fact]
        public void AddGivenPropertyAddsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Add(x => x.TestIntProperty, 200);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/testIntProperty/-", operation.Path);
        }

        [Fact]
        public void AddDeeplyNestedPropertyUsingExpressionAddsCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Add(x => x.Address.Tags, new[] { "NewTag" });

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/address/tags/-", operation.Path);
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
            Assert.Equal("/address/tags/-", operation.Path);
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
            Assert.Equal("/address/zipCode/-", operation.Path);
        }

        [Fact]
        public void AddNestedPropertyUsingPathAddsCorrectValue()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Add("/address/zipCode/-", 22222);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/address/zipCode/-", operation.Path);
        }

        [Fact]
        public void AddGivenNullValueViaExpressionAddsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            // Act
            builder.Add(x => x.TestProperty, null);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/thisIsTheName/-", operation.Path);
        }

        [Fact]
        public void AddEmptyArrayViaExpressionAddsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Add(x => x.Address.Tags, []);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/address/tags/-", operation.Path);
        }

        [Fact]
        public void AddEmptyArrayViaPathAddsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            // Act
            builder.Add<string[]>("/address/tags/-", []);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/address/tags/-", operation.Path);
        }

        [Fact]
        public void AddComplexObjectViaExpressionAddsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            var newAddress = new Address { Street = "New Street", ZipCode = 12345 };

            // Act
            builder.Add(x => x.Address, newAddress);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/address/-", operation.Path);
        }

        [Fact]
        public void AddComplexObjectViaPathAddsCorrectPatchOperation()
        {
            // Arrange
            IPatchOperationBuilder<Person> builder = new PatchOperationBuilder<Person>();

            var newAddress = new Address { Street = "New Street", ZipCode = 12345 };

            // Act
            builder.Add("/address/-", newAddress);

            // Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/address/-", operation.Path);
        }
    }
}
