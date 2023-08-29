// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed;

public class DefaultChangeFeedContainerProcessorProviderTests
{
    private readonly RepositoryOptions _repositoryOptions = new();
    private readonly Mock<ICosmosContainerService> _containerService = new();
    private readonly Mock<ILeaseContainerProvider> _leaseContainerProvider = new();
    private readonly Mock<ILoggerFactory> _loggerFactory = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly Mock<IChangeFeedOptionsProvider> _changeFeedOptionsProvider = new();

    private IChangeFeedContainerProcessorProvider CreateSut()
    {
        Mock<IOptionsMonitor<RepositoryOptions>> options = new();
        options
            .SetupGet(o => o.CurrentValue)
            .Returns(_repositoryOptions);

        return new DefaultChangeFeedContainerProcessorProvider(options.Object,
            _containerService.Object,
            _leaseContainerProvider.Object,
            _loggerFactory.Object,
            _serviceProvider.Object,
            _changeFeedOptionsProvider.Object);
    }

    [Fact]
    public void GetProcessors_ItemSetupForTheChangeFeed_ReturnsCorrectProcessors()
    {
        //Arrange
        IChangeFeedContainerProcessorProvider sut = CreateSut();

        _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder =>
        {
            builder.WithContainer("a");
            builder.WithChangeFeedMonitoring();
        });

        _repositoryOptions.ContainerBuilder.Configure<AnotherTestItem>(builder =>
        {
            builder.WithContainer("a");
            builder.WithChangeFeedMonitoring();
        });

        _repositoryOptions.ContainerBuilder.Configure<AndAnotherItem>(builder =>
        {
            builder.WithContainer("b");
            builder.WithChangeFeedMonitoring();
        });

        _repositoryOptions.ContainerBuilder.Configure<TestItemWithEtag>(builder => builder.WithContainer("c"));

        //Act
        var processors = sut.GetProcessors().ToList();

        //Assert
        Assert.Equal(2, processors.Count);

        IContainerChangeFeedProcessor? aChangeFeedProcessor = processors
            .FirstOrDefault(x =>
                x.ItemTypes.Count is 2 &&
                x.ItemTypes.Contains(typeof(TestItem)) &&
                x.ItemTypes.Contains(typeof(AnotherTestItem)));

        Assert.NotNull(aChangeFeedProcessor);

        IContainerChangeFeedProcessor? bChangeFeedProcessor = processors
            .FirstOrDefault(x =>
                x.ItemTypes.Count is 1 &&
                x.ItemTypes.Contains(typeof(AndAnotherItem)));

        Assert.NotNull(bChangeFeedProcessor);
    }
}