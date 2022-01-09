// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed.InMemory
{
    public class InMemoryChangeFeedTests
    {
        private readonly TestItemProcessor _testItemProcessor;
        private readonly IRepository<TestItem> _testItemRepository;

        public InMemoryChangeFeedTests()
        {
            IServiceProvider provider = new ServiceCollection()
                .AddInMemoryCosmosRepository()
                .AddSingleton<IItemChangeFeedProcessor<TestItem>, TestItemProcessor>()
                .BuildServiceProvider();

            provider.GetRequiredService<InMemoryChangeFeed<TestItem>>().Setup();
            _testItemProcessor = provider.GetRequiredService<IItemChangeFeedProcessor<TestItem>>() as TestItemProcessor;
            _testItemRepository = provider.GetRequiredService<IRepository<TestItem>>();
        }

        [Fact]
        public async Task Create_Item_InvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item = new();

            //Act
            await _testItemRepository.CreateAsync(item);

            //Assert
            Assert.Equal(1, _testItemProcessor.InvocationCount);
        }

        [Fact]
        public async Task Create_CollectionOfItems_InvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item1 = new();
            TestItem item2 = new();
            TestItem item3 = new();

            List<TestItem> items = new List<TestItem>() {item1, item2, item3};

            //Act
            await _testItemRepository.CreateAsync(items);

            //Assert
            Assert.Equal(items.Count, _testItemProcessor.InvocationCount);
        }
    }

    public class TestItemProcessor : IItemChangeFeedProcessor<TestItem>
    {
        public int InvocationCount { get; set; }

        public async ValueTask HandleAsync(TestItem changedItem, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            InvocationCount++;
        }
    }
}