// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddCosmosRepositoryThrowsWithNullServiceCollection() =>
            Assert.Throws<ArgumentNullException>(
                () => (null as IServiceCollection).AddCosmosRepository());
    }
}