﻿// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    using System;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.CosmosRepository.Options;
    using Microsoft.Azure.CosmosRepository.Providers;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class DefaultCosmosClientProviderTests
    {
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
        public void NewDefaultCosmosClientProviderThrowsWithNullCosmosClientOptions() =>
           Assert.Throws<ArgumentNullException>(
               () => new DefaultCosmosClientProvider(
                   null,
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
    }
}
