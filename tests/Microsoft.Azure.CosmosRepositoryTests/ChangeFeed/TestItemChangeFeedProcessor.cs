// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed;

public class TestItemChangeFeedProcessor : IItemChangeFeedProcessor<TestItem>
{
    public int InvocationCount { get; set; }

    public List<TestItem> ChangedItems { get; } = [];

    public async ValueTask HandleAsync(TestItem rating, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        ChangedItems.Add(rating);
        InvocationCount++;
    }
}