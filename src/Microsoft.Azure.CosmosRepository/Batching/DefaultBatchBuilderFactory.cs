namespace Microsoft.Azure.CosmosRepository;

internal sealed class DefaultBatchBuilderFactory(
    ICosmosContainerService containerService) : IBatchBuilderFactory
{
    public IBatchBuilder CreateBatch(string partitionKey) =>
        new DefaultBatchBuilder(partitionKey, containerService);
}
