// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed;

public class DefaultChangeFeedServiceTests
{
    [Fact]
    public async Task StartAsync_StopAsync_EnumeratesProviderProcessorsOnlyOnce()
    {
        //Arrange
        (DefaultChangeFeedService sut, TrackingChangeFeedContainerProcessorProvider provider, _, _) = CreateSut();

        //Act
        await sut.StartAsync(default);
        await sut.StopAsync();

        //Assert
        Assert.Equal(1, provider.GetProcessorsCallCount);
        Assert.Equal(1, provider.EnumerationCount);
    }

    [Fact]
    public async Task StartAsync_StopAsync_StopsTheProcessorInstancesThatWereStarted()
    {
        //Arrange
        (DefaultChangeFeedService sut, _, IReadOnlyList<TrackingContainerChangeFeedProcessor> startedProcessors, IReadOnlyList<TrackingContainerChangeFeedProcessor> replacementProcessors) = CreateSut();

        //Act
        await sut.StartAsync(default);
        await sut.StopAsync();

        //Assert
        foreach (TrackingContainerChangeFeedProcessor processor in startedProcessors)
        {
            Assert.Equal(1, processor.StartCallCount);
            Assert.Equal(1, processor.StopCallCount);
        }

        foreach (TrackingContainerChangeFeedProcessor processor in replacementProcessors)
        {
            Assert.Equal(0, processor.StartCallCount);
            Assert.Equal(0, processor.StopCallCount);
        }
    }

    private static (DefaultChangeFeedService Sut,
        TrackingChangeFeedContainerProcessorProvider Provider,
        IReadOnlyList<TrackingContainerChangeFeedProcessor> StartedProcessors,
        IReadOnlyList<TrackingContainerChangeFeedProcessor> ReplacementProcessors) CreateSut()
    {
        TrackingContainerChangeFeedProcessor[] startedProcessors =
        [
            new(),
            new()
        ];

        TrackingContainerChangeFeedProcessor[] replacementProcessors =
        [
            new(),
            new()
        ];

        TrackingChangeFeedContainerProcessorProvider provider = new(startedProcessors, replacementProcessors);

        return (new DefaultChangeFeedService([provider]), provider, startedProcessors, replacementProcessors);
    }

    private sealed class TrackingChangeFeedContainerProcessorProvider : IChangeFeedContainerProcessorProvider
    {
        private readonly IReadOnlyList<TrackingContainerChangeFeedProcessor> _firstEnumerationProcessors;
        private readonly IReadOnlyList<TrackingContainerChangeFeedProcessor> _secondEnumerationProcessors;
        private readonly IEnumerable<IContainerChangeFeedProcessor> _processors;

        public TrackingChangeFeedContainerProcessorProvider(
            IReadOnlyList<TrackingContainerChangeFeedProcessor> firstEnumerationProcessors,
            IReadOnlyList<TrackingContainerChangeFeedProcessor> secondEnumerationProcessors)
        {
            _firstEnumerationProcessors = firstEnumerationProcessors;
            _secondEnumerationProcessors = secondEnumerationProcessors;
            _processors = EnumerateProcessors();
        }

        public int GetProcessorsCallCount { get; private set; }

        public int EnumerationCount { get; private set; }

        public IEnumerable<IContainerChangeFeedProcessor> GetProcessors()
        {
            GetProcessorsCallCount++;

            return _processors;
        }

        private IEnumerable<IContainerChangeFeedProcessor> EnumerateProcessors()
        {
            EnumerationCount++;

            IReadOnlyList<TrackingContainerChangeFeedProcessor> processors = EnumerationCount switch
            {
                1 => _firstEnumerationProcessors,
                2 => _secondEnumerationProcessors,
                _ => _secondEnumerationProcessors
            };

            foreach (TrackingContainerChangeFeedProcessor processor in processors)
            {
                yield return processor;
            }
        }
    }

    private sealed class TrackingContainerChangeFeedProcessor : IContainerChangeFeedProcessor
    {
        public int StartCallCount { get; private set; }

        public int StopCallCount { get; private set; }

        public IReadOnlyList<Type> ItemTypes { get; } = [typeof(TestItem)];

        public Task StartAsync()
        {
            StartCallCount++;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            StopCallCount++;

            return Task.CompletedTask;
        }
    }
}
