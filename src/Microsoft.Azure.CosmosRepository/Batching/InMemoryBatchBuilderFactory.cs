namespace Microsoft.Azure.CosmosRepository;

internal sealed class InMemoryBatchBuilderFactory : IBatchBuilderFactory
{
    public IBatchBuilder CreateBatch(string partitionKey) =>
        new InMemoryBatchBuilder(partitionKey);
}
