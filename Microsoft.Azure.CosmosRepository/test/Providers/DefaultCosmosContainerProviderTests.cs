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

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullCosmosClient() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    null,
                    new DefaultCosmosPartitionKeyPathProvider(_options.Object),
                    new DefaultCosmosContainerNameProvider(_options.Object),
                    new DefaultCosmosUniqueKeyPolicyProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullCosmosPartitionKeyNameProvider() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    null,
                    new DefaultCosmosContainerNameProvider(_options.Object),
                    new DefaultCosmosUniqueKeyPolicyProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullCosmosContainerNameProvider() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(_options.Object),
                    null,
                    new DefaultCosmosUniqueKeyPolicyProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullUniqueKeyPolicyProvider() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(_options.Object),
                    new DefaultCosmosContainerNameProvider(_options.Object),
                    null,
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        DatabaseId = "data",
                        ContainerId = "container"
                    }),
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullOptions() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(_options.Object),
                    new DefaultCosmosContainerNameProvider(_options.Object),
                    new DefaultCosmosUniqueKeyPolicyProvider(),
                    null,
                    _loggerFactory.CreateLogger<DefaultCosmosContainerProvider<TestItem>>()));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullConnectionString() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(_options.Object),
                    new DefaultCosmosContainerNameProvider(_options.Object),
                    new DefaultCosmosUniqueKeyPolicyProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        DatabaseId = "data",
                        ContainerId = "container"
                    }), null));
        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullDatabaseId() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(_options.Object),
                    new DefaultCosmosContainerNameProvider(_options.Object),
                    new DefaultCosmosUniqueKeyPolicyProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        ContainerId = "container"
                    }), null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullContainerId() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(_options.Object),
                    new DefaultCosmosContainerNameProvider(_options.Object),
                    new DefaultCosmosUniqueKeyPolicyProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data"
                    }), null));

        [Fact]
        public void NewDefaultCosmosContainerProviderThrowsWithNullLogger() =>
            Assert.Throws<ArgumentNullException>(
                () => new DefaultCosmosContainerProvider<TestItem>(
                    new TestCosmosClientProvider(),
                    new DefaultCosmosPartitionKeyPathProvider(_options.Object),
                    new DefaultCosmosContainerNameProvider(_options.Object),
                    new DefaultCosmosUniqueKeyPolicyProvider(),
                    Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                    {
                        CosmosConnectionString = "pickles",
                        DatabaseId = "data",
                        ContainerId = "container"
                    }), null));
    }

    public class TestItem : Item { }

    internal class TestCosmosClientProvider : ICosmosClientProvider
    {
        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) =>
            Task.FromResult(default(T));
    }
}