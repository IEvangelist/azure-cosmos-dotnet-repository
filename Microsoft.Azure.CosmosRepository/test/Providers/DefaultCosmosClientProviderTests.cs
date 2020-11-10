// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosClientProviderTests
    {
        [Fact]
        public void NewDefaultCosmosClientProviderThrowsWithNullCosmosClientOptions() =>
           Assert.Throws<ArgumentNullException>(
               () => new DefaultCosmosClientProvider(
                                                     cosmosClientOptions: null,
                                                     Options.Create(new RepositoryOptions
                                                     {
                                                         CosmosConnectionString = "pickles",
                                                         DatabaseId = "data",
                                                         ContainerId = "container"
                                                     })));

        [Fact]
        public void NewDefaultCosmosClientProviderThrowsWithNullCosmosClientOptionsProvider() =>
           Assert.Throws<ArgumentNullException>(
               () => new DefaultCosmosClientProvider(
                                                     cosmosClientOptionsProvider: null,
                                                     Options.Create(new RepositoryOptions
                                                     {
                                                         CosmosConnectionString = "pickles",
                                                         DatabaseId = "data",
                                                         ContainerId = "container"
                                                     })));

        [Fact]
        public void NewDefaultCosmosClientProviderThrowsWithNullRepositoryOptions() =>
           Assert.Throws<ArgumentNullException>(
               () => new DefaultCosmosClientProvider(
                                                     new CosmosClientOptions(),
                                                     null));

        [Fact]
        public void NewDefaultCosmosClientProviderThrowsWithNullRepositoryOptionsOverload() =>
           Assert.Throws<ArgumentNullException>(
               () => new DefaultCosmosClientProvider(
                                                     new Mock<ICosmosClientOptionsProvider>().Object,
                                                     null));

        [Fact]
        public void DefaultCosmosClientProviderCorrectlyDisposes()
        {
            DefaultCosmosClientProvider provider =
                new DefaultCosmosClientProvider(
                                                new CosmosClientOptions(),
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString =
                            "AccountEndpoint=https://localtestcosmos.documents.azure.com:443/;AccountKey=RmFrZUtleQ==;"
                    }));

            provider.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await provider.UseClientAsync(client => client.ReadAccountAsync()));
        }

        [Fact]
        public void DefaultCosmosClientProviderCorrectlyDisposesOverload()
        {
            DefaultCosmosClientProvider provider =
                new DefaultCosmosClientProvider(
                    new Mock<ICosmosClientOptionsProvider>().Object,
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString =
                            "AccountEndpoint=https://localtestcosmos.documents.azure.com:443/;AccountKey=RmFrZUtleQ==;"
                    }));

            provider.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await provider.UseClientAsync(client => client.ReadAccountAsync()));
        }
    }
}
