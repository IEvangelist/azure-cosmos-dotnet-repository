// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.VisualBasic.CompilerServices;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Options
{
    public class RepositoryOptionsTests
    {
        [Fact]
        public void RepositoryOptionsBuilderConfiguresItemCorrectly()
        {
            RepositoryOptions options = new();
            options.ContainerBuilder.Configure<Product>(
                containerOptions => containerOptions
                    .WithContainer("products")
                    .WithPartitionKey("/category")
            );

            Assert.Equal(1, options.ContainerOptions.Count);
            Assert.Equal("products", options.ContainerOptions[0].Name);
            Assert.Equal("/category", options.ContainerOptions[0].PartitionKey);
        }

        [Fact]
        public void RepositoryOptionsBuilderThrowsArgumentNullExceptionWhenContainerNameIsNull()
            => Assert.Throws<ArgumentNullException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(options => options.WithContainer(null)));

        [Fact]
        public void RepositoryOptionsBuilderThrowsArgumentNullExceptionWhenPartionKeyIsNull()
            => Assert.Throws<ArgumentNullException>(() => new RepositoryOptions().ContainerBuilder.Configure<Product>(options => options.WithPartitionKey(null)));
    }

    public class Product : Item
    {

    }
}