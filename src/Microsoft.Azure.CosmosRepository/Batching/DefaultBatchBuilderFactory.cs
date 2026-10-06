// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

internal sealed class DefaultBatchBuilderFactory(
    ICosmosContainerService containerService) : IBatchBuilderFactory
{
    public IBatchBuilder CreateBatch(string partitionKey) =>
        new DefaultBatchBuilder(
            partitionKey ?? throw new ArgumentNullException(nameof(partitionKey)),
            containerService);
}
