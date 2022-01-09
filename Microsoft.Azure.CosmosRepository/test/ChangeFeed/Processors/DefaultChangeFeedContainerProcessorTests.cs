// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed.Processors
{
    public class DefaultChangeFeedContainerProcessorTests
    {
        private readonly Mock<ICosmosContainerService> _containerService = new();
        private readonly Mock<ILeaseContainerProvider> _leaseContainerProvider = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly List<Type> _itemTypes = new() {typeof(TestItem), typeof(AnotherTestItem)};
        private readonly TestItemChangeFeedProcessor _testItemChangeFeedChangeFeedProcessor;
        private readonly ChangeFeedOptions _changeFeedOptions = new(typeof(TestItem));

        public DefaultChangeFeedContainerProcessorTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddCosmosRepositoryItemChangeFeedProcessors(Assembly.GetExecutingAssembly())
                .BuildServiceProvider();

            _testItemChangeFeedChangeFeedProcessor = _serviceProvider.GetRequiredService<IItemChangeFeedProcessor<TestItem>>() as TestItemChangeFeedProcessor;
        }

        private DefaultContainerChangeFeedProcessor CreateSut() =>
            new(_containerService.Object,
                _itemTypes,
                _leaseContainerProvider.Object,
                _changeFeedOptions,
                new NullLogger<DefaultContainerChangeFeedProcessor>(),
                _serviceProvider);

        [Fact]
        public async Task OnChanges_ItemThatHasChanged_InvokesHandler()
        {
            //Arrange
            DefaultContainerChangeFeedProcessor sut = CreateSut();

            TestItem item = new() {Property = "a"};

            List<JObject> changes = new() {JObject.FromObject(item)};

            //Act
            await sut.OnChanges(changes, default, "test");

            //Assert
            Assert.Equal(1, _testItemChangeFeedChangeFeedProcessor.InvocationCount);
        }

        [Fact]
        public async Task OnChanges_ItemThatHasNoRegisteredType_DoesNotInvokesHandler()
        {
            //Arrange
            DefaultContainerChangeFeedProcessor sut = CreateSut();

            List<JObject> changes = new() {JObject.FromObject(new {type = "IAmNotSetup"})};

            //Act
            await sut.OnChanges(changes, default, "test");

            //Assert
            Assert.Equal(0, _testItemChangeFeedChangeFeedProcessor.InvocationCount);
        }

        [Fact]
        public async Task OnChanges_TypeFieldNotFound_DoesNotInvokesHandler()
        {
            //Arrange
            DefaultContainerChangeFeedProcessor sut = CreateSut();

            List<JObject> changes = new() {JObject.FromObject(new {id = "a"})};

            //Act
            await sut.OnChanges(changes, default, "test");

            //Assert
            Assert.Equal(0, _testItemChangeFeedChangeFeedProcessor.InvocationCount);
        }
    }
}