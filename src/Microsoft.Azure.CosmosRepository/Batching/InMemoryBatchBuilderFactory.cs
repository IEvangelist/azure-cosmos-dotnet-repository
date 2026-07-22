// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

internal sealed class InMemoryBatchBuilderFactory(
    IServiceProvider serviceProvider) : IBatchBuilderFactory
{
    public IBatchBuilder CreateBatch(string partitionKey) =>
        new InMemoryBatchBuilder(partitionKey, serviceProvider);
}
