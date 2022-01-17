// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed
{
    public class TestItemChangeFeedProcessor : IItemChangeFeedProcessor<TestItem>
    {
        public int InvocationCount { get; set; }

        public List<TestItem> ChangedItems { get; } = new();

        public async ValueTask HandleAsync(TestItem rating, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            ChangedItems.Add(rating);
            InvocationCount++;
        }
    }
}