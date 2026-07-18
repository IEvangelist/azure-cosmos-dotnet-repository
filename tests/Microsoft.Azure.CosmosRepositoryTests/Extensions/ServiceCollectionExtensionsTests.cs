// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCosmosRepositoryThrowsWithNullServiceCollection() =>
        Assert.Throws<ArgumentNullException>(
            () => (null as IServiceCollection)!.AddCosmosRepository());

    [Fact]
    public void AddInMemoryCosmosRepositoryThrowsWithNullServiceCollection() =>
        Assert.Throws<ArgumentNullException>(
            () => (null as IServiceCollection)!.AddInMemoryCosmosRepository());

    [Fact]
    public void AddInMemoryCosmosRepositoryRegistersBatchRepository()
    {
        IServiceProvider provider = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .BuildServiceProvider();

        IBatchRepository<TestItem> repository = provider.GetRequiredService<IBatchRepository<TestItem>>();

        Assert.NotNull(repository);
    }

    [Fact]
    public void AddCosmosRepositoryRegistersBatchBuilderFactory()
    {
        IServiceCollection services = new ServiceCollection().AddCosmosRepository();

        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IBatchBuilderFactory) &&
            descriptor.ImplementationType == typeof(DefaultBatchBuilderFactory));
    }

    [Fact]
    public void AddInMemoryCosmosRepositoryRegistersBatchBuilderFactory()
    {
        IServiceProvider provider = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .BuildServiceProvider();

        IBatchBuilderFactory factory = provider.GetRequiredService<IBatchBuilderFactory>();

        Assert.NotNull(factory.CreateBatch("pk"));
    }
}
