// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosRepository.AspNetCore;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

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
    [Fact]
    public void AddCosmosRepositoryChangeFeedHostedService_AddsHostedService()
    {
        //Arrange
        ServiceCollection services = new();
        services.AddSingleton(Mock.Of<IChangeFeedService>());

        //Act
        services.AddCosmosRepositoryChangeFeedHostedService();

        //Assert
        using ServiceProvider provider = services.BuildServiceProvider();
        CosmosRepositoryChangeFeedHostedService hostedService =
            provider.GetServices<IHostedService>()
                .OfType<CosmosRepositoryChangeFeedHostedService>()
                .Single();

        Assert.NotNull(hostedService);
    }

    [Fact]
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
