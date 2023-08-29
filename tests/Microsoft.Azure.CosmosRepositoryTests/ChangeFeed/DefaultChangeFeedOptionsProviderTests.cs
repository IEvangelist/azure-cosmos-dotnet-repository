// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed;

public class DefaultChangeFeedOptionsProviderTests
{
    private readonly RepositoryOptions _repositoryOptions = new();

    private IChangeFeedOptionsProvider CreateSut()
    {
        Mock<IOptionsMonitor<RepositoryOptions>> monitor = new();
        monitor.SetupGet(o => o.CurrentValue)
            .Returns(_repositoryOptions);

        return new DefaultChangeFeedOptionsProvider(monitor.Object);
    }

    [Fact]
    public void GetOptionsForItems_ItemTypes_GetsChangeFeedOptions()
    {
        //Arrange
        IChangeFeedOptionsProvider sut = CreateSut();

        _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder => builder.WithChangeFeedMonitoring());

        _repositoryOptions.ContainerBuilder.Configure<AnotherTestItem>(builder => builder.WithChangeFeedMonitoring());

        //Act
        ChangeFeedOptions options = sut.GetOptionsForItems(new[] { typeof(TestItem), typeof(AnotherTestItem) });

        //Assert
        Assert.Equal("default", options.InstanceName);
    }

    [Fact]
    public void GetOptionsForItems_ItemTypesWithDifferentChangeFeedOptions_ThrowsMissMatchedChangeFeedOptionsException()
    {
        //Arrange
        IChangeFeedOptionsProvider sut = CreateSut();

        _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder => builder.WithChangeFeedMonitoring());

        _repositoryOptions.ContainerBuilder.Configure<AnotherTestItem>(builder => builder.WithChangeFeedMonitoring(options => options.InstanceName = "different"));

        IReadOnlyList<Type> types = new[] { typeof(TestItem), typeof(AnotherTestItem) };

        //Act
        //Assert
        Assert.Throws<MissMatchedChangeFeedOptionsException>(() =>
            sut.GetOptionsForItems(types));
    }
}