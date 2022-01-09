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
        public async Task CreateAsync_Item_InvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item = new();

            //Act
            await _testItemRepository.CreateAsync(item);

            //Assert
            Assert.Equal(1, _testItemProcessor.InvocationCount);
        }

        [Fact]
        public async Task CreateAsync_ItemThatConflicts_DoesNotInvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item = new();

            await _testItemRepository.CreateAsync(item);

            //Act
            try
            {
                await _testItemRepository.CreateAsync(item);
            }
            catch
            {
                // ignored
            }

            //Assert
            Assert.Equal(1, _testItemProcessor.InvocationCount);
        }

        [Fact]
        public async Task CreateAsync_CollectionOfItems_InvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item1 = new();
            TestItem item2 = new();
            TestItem item3 = new();

            List<TestItem> items = new() {item1, item2, item3};

            //Act
            await _testItemRepository.CreateAsync(items);

            //Assert
            Assert.Equal(items.Count, _testItemProcessor.InvocationCount);
        }

        [Fact]
        public async Task UpdateAsync_Item_InvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item = new();

            //Act
            await _testItemRepository.UpdateAsync(item);

            //Assert
            Assert.Equal(1, _testItemProcessor.InvocationCount);
        }

        [Fact]
        public async Task UpdateAsync_ItemThatExists_InvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item = new();

            item = await _testItemRepository.CreateAsync(item);

            //Act
            await _testItemRepository.UpdateAsync(item);

            //Assert
            Assert.Equal(2, _testItemProcessor.InvocationCount);
        }

        [Fact]
        public async Task UpdateAsync_CollectionOfItemWhereSomeExist_InvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item1 = new();
            TestItem item2 = new();
            TestItem item3 = new();

            item1 = await _testItemRepository.CreateAsync(item1);
            item3 = await _testItemRepository.CreateAsync(item3);

            List<TestItem> items = new() {item1, item2, item3};

            //Act
            await _testItemRepository.UpdateAsync(items);

            //Assert
            Assert.Equal(5, _testItemProcessor.InvocationCount);
        }

        [Fact]
        public async Task UpdateAsync_PatchUpdate_InvokesChangeFeedProcessor()
        {
            //Arrange
            TestItem item = new();

            item = await _testItemRepository.CreateAsync(item);

            //Act
            await _testItemRepository.UpdateAsync(item.Id, builder => builder.Replace(x => x.Property, "propertyValue"));

            //Assert
            Assert.Equal(2, _testItemProcessor.InvocationCount);
            Assert.Contains(_testItemProcessor.ChangedItems, x => x.Property == "propertyValue");
        }

    }

    public class TestItemProcessor : IItemChangeFeedProcessor<TestItem>
    {
        public int InvocationCount { get; set; }

        public List<TestItem> ChangedItems { get; } = new();

        public async ValueTask HandleAsync(TestItem changedItem, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            ChangedItems.Add(changedItem);
            InvocationCount++;
        }
    }
}