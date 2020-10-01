using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosContainerProviderTests
    {
        readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullCosmosClientOptions() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider(
                    null,
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider>()));


        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullOptions() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider(
                    new CosmosClientOptions(),
                    null,
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullConnectionString() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider(
                    new CosmosClientOptions(),
                    Options.Create(new RepositoryOptions
                    {
                        DatabaseId = "data",
                        ContainerId = "container"
                    }), null));
        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullDatabaseId() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider(
                    new CosmosClientOptions(),
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        ContainerId = "container"
                    }), null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullContainerId() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider(
                    new CosmosClientOptions(),
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data"
                    }), null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullLogger() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider(
                    new CosmosClientOptions(),
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data",
                        ContainerId = "container"
                    }), null));
    }
}