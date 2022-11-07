// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Builders
{
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

    public class PatchOperationBuilderTests
    {
        [Fact]
        public void GetPropertyToReplaceGetsCorrectValueWithJsonAttribute()
        {
            //Arrange
            PatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();
            Expression<Func<Item1, string>> expression = item => item.TestProperty;

            //Act
            string path = builder.GetPropertyToReplace(expression);

            //Assert
            Assert.Equal("thisIsTheName", path);
        }

        [Fact]
        public void GetPropertyToReplaceGetsCorrectValueWithNoAttributes()
        {
            //Arrange
            PatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();
            Expression<Func<Item1, int>> expression = item => item.TestIntProperty;

            //Act
            string path = builder.GetPropertyToReplace(expression);

            //Assert
            Assert.Equal("testIntProperty", path);
        }

        [Fact]
        public void GetPropertyToReplaceGetsCorrectValueWithRequiredAttribute()
        {
            //Arrange
            PatchOperationBuilder<RequiredItem> builder = new PatchOperationBuilder<RequiredItem>();
            Expression<Func<RequiredItem, string>> expression = item => item.TestProperty;

            //Act
            string path = builder.GetPropertyToReplace(expression);

            //Assert
            Assert.Equal("testProperty", path);
        }

        [Fact]
        public void GetPropertyToReplaceGetsCorrectValueWithRequiredAndJsonAttribute()
        {
            //Arrange
            PatchOperationBuilder<RequiredAndJsonItem> builder = new PatchOperationBuilder<RequiredAndJsonItem>();
            Expression<Func<RequiredAndJsonItem, string>> expression = item => item.TestProperty;

            //Act
            string path = builder.GetPropertyToReplace(expression);

            //Assert
            Assert.Equal("testProperty", path);
        }

        [Fact]
        public void ReplaceGivenExpressionSetsCorrectPatchOperation()
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
        public void ReplaceGivenPathSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Replace("thisIsTheName", "100");

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Replace, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void SetGivenExpressionSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Set(x => x.TestProperty, "100");

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void SetGivenPathSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Set("thisIsTheName", "100");

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Set, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void AddGivenExpressionSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Add(x => x.TestProperty, "100");

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void AddGivenPathSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Add("thisIsTheName", "100");

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Add, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void RemoveGivenExpressionSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Remove(x => x.TestProperty);

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Remove, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void RemoveGivenPathSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Remove("thisIsTheName");

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Remove, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void IncrementDoubleGivenExpressionSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Increment(x => x.TestProperty, 100.123);

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void IncrementDoubleGivenPathSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Increment("thisIsTheName", 100.123);

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void IncrementLongGivenExpressionSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Increment(x => x.TestProperty, 123456789);

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
        }

        [Fact]
        public void IncrementLongGivenPathSetsCorrectPatchOperation()
        {
            //Arrange
            IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

            //Act
            builder.Increment("thisIsTheName", 123456789);

            //Assert
            PatchOperation operation = builder.PatchOperations[0];
            Assert.Equal(PatchOperationType.Increment, operation.OperationType);
            Assert.Equal("/thisIsTheName", operation.Path);
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
}