// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepositoryTests.Abstractions;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Providers
{
    public class DefaultCosmosContainerDefaultTimeToLiveProviderTests : WithRepositoryOptions
    {
        readonly DefaultCosmosContainerDefaultTimeToLiveProvider _provider;

        public DefaultCosmosContainerDefaultTimeToLiveProviderTests() =>
            _provider = new DefaultCosmosContainerDefaultTimeToLiveProvider(_options.Object);

        [Fact]
        public void GetDefaultTimeToLiveItemWithNoOptionsReturnsMinus1() =>
            Assert.Equal(-1, _provider.GetDefaultTimeToLive<TestItem>());

        [Fact]
        public void GetDefaultTimeToLiveItemWithOptionsUsesThatTimeToLive()
        {
            TimeSpan timeToLive = TimeSpan.FromMinutes(5);
            _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder =>
                builder.WithContainerDefaultTimeToLive(timeToLive));
        }

        [Fact]
        public void GetDefaultTimeToLiveItemWithOptionsThatShareAContainerNameWithEqualValues()
        {
            _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder =>
                builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(5)).WithContainer("a"));

            _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder =>
                builder.WithContainer("a"));

            Assert.Equal(5 * 60, _provider.GetDefaultTimeToLive<TestItem>());
        }

        [Fact]
        public void GetDefaultTimeToLiveItemWithOptionsThatShareAContainerNameAndHaveConflictingValuesThrows()
        {
            _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder =>
                builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(5)).WithContainer("a"));

            _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder =>
                builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(6)).WithContainer("a"));

            Assert.Throws<InvalidOperationException>(() => _provider.GetDefaultTimeToLive<TestItem>());
        }
    }
}