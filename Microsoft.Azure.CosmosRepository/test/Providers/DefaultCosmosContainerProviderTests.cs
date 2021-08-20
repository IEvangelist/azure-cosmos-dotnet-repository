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
    }

    public class TestItem : Item { }

    internal class TestCosmosClientProvider : ICosmosClientProvider
    {
        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) =>
            Task.FromResult(default(T));
    }
}