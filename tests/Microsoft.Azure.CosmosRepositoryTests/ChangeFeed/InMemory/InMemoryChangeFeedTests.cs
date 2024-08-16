// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed.InMemory;

public class InMemoryChangeFeedTests
{
    private readonly TestItemChangeFeedProcessor _testItemChangeFeedProcessor;
    private readonly IRepository<TestItem> _testItemRepository;

    public InMemoryChangeFeedTests()
    {
        IServiceProvider provider = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .AddSingleton<IItemChangeFeedProcessor<TestItem>, TestItemChangeFeedProcessor>()
            .BuildServiceProvider();

        provider.GetRequiredService<InMemoryChangeFeed<TestItem>>().Setup();
        _testItemChangeFeedProcessor =
            (provider.GetRequiredService<IItemChangeFeedProcessor<TestItem>>() as TestItemChangeFeedProcessor)!;

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
        Assert.Equal(1, _testItemChangeFeedProcessor.InvocationCount);
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
        Assert.Equal(1, _testItemChangeFeedProcessor.InvocationCount);
    }

    [Fact]
    public async Task CreateAsync_CollectionOfItems_InvokesChangeFeedProcessor()
    {
        //Arrange
        TestItem item1 = new();
        TestItem item2 = new();
        TestItem item3 = new();

        List<TestItem> items = [item1, item2, item3];

        //Act
        await _testItemRepository.CreateAsync(items);

        //Assert
        Assert.Equal(items.Count, _testItemChangeFeedProcessor.InvocationCount);
    }

    [Fact]
    public async Task UpdateAsync_Item_InvokesChangeFeedProcessor()
    {
        //Arrange
        TestItem item = new();

        //Act
        await _testItemRepository.UpdateAsync(item);

        //Assert
        Assert.Equal(1, _testItemChangeFeedProcessor.InvocationCount);
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
        Assert.Equal(2, _testItemChangeFeedProcessor.InvocationCount);
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

        List<TestItem> items = [item1, item2, item3];

        //Act
        await _testItemRepository.UpdateAsync(items);

        //Assert
        Assert.Equal(5, _testItemChangeFeedProcessor.InvocationCount);
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
        Assert.Equal(2, _testItemChangeFeedProcessor.InvocationCount);
        Assert.Contains(_testItemChangeFeedProcessor.ChangedItems, x => x.Property == "propertyValue");
    }

    //TODO: Add more tests for other operations Set, Add, Remove, Increment, 

}