// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed;

public class DefaultChangeFeedOptionsProviderTests
{
    private IChangeFeedOptionsProvider CreateSut(Func<RepositoryOptions> getCurrentOptions)
    {
        Mock<IOptionsMonitor<RepositoryOptions>> monitor = new();
        monitor.SetupGet(o => o.CurrentValue)
            .Returns(getCurrentOptions);

        return new DefaultChangeFeedOptionsProvider(monitor.Object);
    }

    [Fact]
    public void GetOptionsForItems_ItemTypes_GetsChangeFeedOptions()
    {
        //Arrange
        RepositoryOptions repositoryOptions = new();
        IChangeFeedOptionsProvider sut = CreateSut(() => repositoryOptions);

        repositoryOptions.ContainerBuilder.Configure<TestItem>(builder => builder.WithChangeFeedMonitoring());

        repositoryOptions.ContainerBuilder.Configure<AnotherTestItem>(builder => builder.WithChangeFeedMonitoring());

        //Act
        ChangeFeedOptions options = sut.GetOptionsForItems(new[] { typeof(TestItem), typeof(AnotherTestItem) });

        //Assert
        Assert.Equal("default", options.InstanceName);
    }

    [Fact]
    public void GetOptionsForItems_ItemTypesWithDifferentChangeFeedOptions_ThrowsMissMatchedChangeFeedOptionsException()
    {
        //Arrange
        RepositoryOptions repositoryOptions = new();
        IChangeFeedOptionsProvider sut = CreateSut(() => repositoryOptions);

        repositoryOptions.ContainerBuilder.Configure<TestItem>(builder => builder.WithChangeFeedMonitoring());

        repositoryOptions.ContainerBuilder.Configure<AnotherTestItem>(builder => builder.WithChangeFeedMonitoring(options => options.InstanceName = "different"));

        IReadOnlyList<Type> types = new[] { typeof(TestItem), typeof(AnotherTestItem) };

        //Act
        //Assert
        Assert.Throws<MissMatchedChangeFeedOptionsException>(() =>
            sut.GetOptionsForItems(types));
    }

    [Fact]
    public void GetOptionsForItems_UsesCurrentOptionsValueAtCallTime()
    {
        //Arrange
        RepositoryOptions initialOptions = new();
        initialOptions.ContainerBuilder.Configure<TestItem>(builder => builder.WithChangeFeedMonitoring(options => options.InstanceName = "initial"));

        RepositoryOptions updatedOptions = new();
        updatedOptions.ContainerBuilder.Configure<TestItem>(builder => builder.WithChangeFeedMonitoring(options => options.InstanceName = "updated"));

        RepositoryOptions currentOptions = initialOptions;
        IChangeFeedOptionsProvider sut = CreateSut(() => currentOptions);

        currentOptions = updatedOptions;

        //Act
        ChangeFeedOptions options = sut.GetOptionsForItems(new[] { typeof(TestItem) });

        //Assert
        Assert.Equal("updated", options.InstanceName);
    }
}
