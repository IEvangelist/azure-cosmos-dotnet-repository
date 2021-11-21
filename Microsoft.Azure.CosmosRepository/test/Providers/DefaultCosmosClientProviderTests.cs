// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosClientProviderTests
    {
        [Fact]
        public void NewDefaultCosmosClientProviderThrowsWithNullCosmosClientOptionsProviderAndValidConnectionStringOptions() =>
           Assert.Throws<ArgumentNullException>(
               () => new DefaultCosmosClientProvider(
                   cosmosClientOptionsProvider: null,
                   Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                   {
                       CosmosConnectionString = "pickles",
                       DatabaseId = "data",
                       ContainerId = "container"
                   })));

        [Fact]
        public void NewDefaultCosmosClientProviderThrowsWithNullCosmosClientOptionsProviderAndValidTokenCredentialOptions() =>
           Assert.Throws<ArgumentNullException>(
               () => new DefaultCosmosClientProvider(
                   cosmosClientOptionsProvider: null,
                   Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                   {
                       TokenCredential = new TestTokenCredential(),
                       AccountEndpoint = "pickles endpoint",
                       DatabaseId = "data",
                       ContainerId = "container"
                   })));

        [Fact]
        public void NewDefaultCosmosClientProviderThrowsWithNullRepositoryOptionsOverload() =>
           Assert.Throws<ArgumentNullException>(
               () => new DefaultCosmosClientProvider(
                   new Mock<ICosmosClientOptionsProvider>().Object,
                   null));

        [Fact]
        public void DefaultCosmosClientProviderCorrectlyDisposesOverloadWithConnectionString()
        {
            Mock<ICosmosClientOptionsProvider> mock = new Mock<ICosmosClientOptionsProvider>();
            mock.SetupGet(provider => provider.ClientOptions).Returns(new CosmosClientOptions());
            DefaultCosmosClientProvider provider =
                new DefaultCosmosClientProvider(
                    mock.Object,
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString =
                            "AccountEndpoint=https://localtestcosmos.documents.azure.com:443/;AccountKey=RmFrZUtleQ==;"
                    }));

            provider.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await provider.UseClientAsync(client => client.ReadAccountAsync()));
        }

        [Fact]
        public void DefaultCosmosClientProviderCorrectlyDisposesOverloadWithTokenCredential()
        {
            Mock<ICosmosClientOptionsProvider> mock = new Mock<ICosmosClientOptionsProvider>();
            mock.SetupGet(provider => provider.ClientOptions).Returns(new CosmosClientOptions());
            DefaultCosmosClientProvider provider =
                new DefaultCosmosClientProvider(
                    mock.Object,
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        TokenCredential = new TestTokenCredential(),
                        AccountEndpoint = "https://localtestcosmos.documents.azure.com:443/"
                    }));

            provider.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await provider.UseClientAsync(client => client.ReadAccountAsync()));
        }
    }
}
