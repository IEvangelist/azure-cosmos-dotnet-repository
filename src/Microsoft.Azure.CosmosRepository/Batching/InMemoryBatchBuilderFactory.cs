namespace Microsoft.Azure.CosmosRepository;

internal sealed class InMemoryBatchBuilderFactory(
    IServiceProvider serviceProvider) : IBatchBuilderFactory
{
    public IBatchBuilder CreateBatch(string partitionKey) =>
        new InMemoryBatchBuilder(partitionKey, serviceProvider);
}
