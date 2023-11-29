// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.AspNetCoreTests.Extensions;

public class TestItem1 : Item
{

}

public class TestItem2 : Item
{

}

public class Processor1 : IItemChangeFeedProcessor<TestItem1>
{
    public ValueTask HandleAsync(TestItem1 rating, CancellationToken cancellationToken) => throw new System.NotImplementedException();
}

public class Processor2 : IItemChangeFeedProcessor<TestItem2>
{
    public ValueTask HandleAsync(TestItem2 rating, CancellationToken cancellationToken) => throw new System.NotImplementedException();
}

public class ServiceCollectionExtensionsTests
{
    public void AddCosmosRepositoryItemChangeFeedProcessors_AssemblyAddsProcessors()
    {
        //Arrange
        ServiceCollection services = new();

        //Act
        services.AddCosmosRepositoryItemChangeFeedProcessors(Assembly.GetExecutingAssembly());

        //Assert
        ServiceProvider? provider = services.BuildServiceProvider();
        provider.GetRequiredService<IItemChangeFeedProcessor<TestItem1>>();
        provider.GetRequiredService<IItemChangeFeedProcessor<TestItem2>>();
    }
}