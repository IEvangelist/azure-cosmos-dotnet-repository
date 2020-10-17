// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.CosmosRepository;
    using Microsoft.Azure.CosmosRepository.Options;
    using Microsoft.Azure.CosmosRepository.Providers;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class DefaultCosmosContainerProviderTests
    {
        private readonly ILoggerFactory loggerFactory = new LoggerFactory();

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullConnectionString() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(),
                    Options.Create(
                        new RepositoryOptions
                        {
                            DatabaseId = "data",
                            ContainerId = "container"
                        }),
                    null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullContainerId() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(),
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data"
                    }), null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullCosmosClient() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    null,
                    new DefaultCosmosPartitionKeyPathProvider(),
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    this.loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullCosmosPartitionKeyNameProvider() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    null,
                    Options.Create(new RepositoryOptions
                    {
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    this.loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullDatabaseId() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(),
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        ContainerId = "container"
                    }), null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullLogger() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(),
                    Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data",
                        ContainerId = "container"
                    }), null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullOptions() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(),
                    null,
                    this.loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));
    }

    public class TestItem : Item { }

    internal class TestCosmosClientProvider : ICosmosClientProvider
    {
        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) =>
            Task.FromResult(default(T));
    }
}
