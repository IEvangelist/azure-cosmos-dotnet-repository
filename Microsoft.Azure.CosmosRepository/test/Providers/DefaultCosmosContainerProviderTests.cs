// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosContainerProviderTests
    {
        readonly ILoggerFactory _loggerFactory = new LoggerFactory();
        readonly Mock<IOptions<RepositoryOptions>> _options = new ();
        private readonly Mock<ICosmosItemConfigurationProvider> _itemConfigurationProvider = new();

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullCosmosClient() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    null,
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    _itemConfigurationProvider.Object,
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));



        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullOptions() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    null,
                    _itemConfigurationProvider.Object,
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullConnectionString() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    _itemConfigurationProvider.Object,
                    null));
        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullDatabaseId() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        ContainerId = "container"
                    }),
                    _itemConfigurationProvider.Object,
                    null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullContainerId() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),

                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data"
                    }),
                    _itemConfigurationProvider.Object,
                    null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullLogger() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    _itemConfigurationProvider.Object,
                    null));
    }

    public class TestItem : Item { }

    internal class TestCosmosClientProvider : ICosmosClientProvider
    {
        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) =>
            Task.FromResult(default(T));
    }
}