// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Extensions
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddCosmosRepositoryThrowsWithNullConfiguration() =>
            Assert.Throws<ArgumentNullException>(
                () => new Mock<IServiceCollection>().Object.AddCosmosRepository(null));

        [Fact]
        public void AddCosmosRepositoryThrowsWithNullServiceCollection() =>
            Assert.Throws<ArgumentNullException>(
                () => (null as IServiceCollection).AddCosmosRepository(
                    new Mock<IConfiguration>().Object));
    }
}
