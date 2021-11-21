// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Builders;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Builders
{
    public class Item1 : Item
    {
        [JsonProperty("thisIsTheName")]
        public string TestProperty { get; set; }

        public int TestIntProperty { get; set; }
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
            PatchOperation operation = builder.PatchOperations.First();
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
            PatchOperation operation = builder.PatchOperations.First();
            Assert.Equal(PatchOperationType.Replace, operation.OperationType);
            Assert.Equal("/testIntProperty", operation.Path);

        }
    }
}