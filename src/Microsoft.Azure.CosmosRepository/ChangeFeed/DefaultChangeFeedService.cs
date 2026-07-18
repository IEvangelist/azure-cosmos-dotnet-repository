// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed;

class DefaultChangeFeedService(IEnumerable<IChangeFeedContainerProcessorProvider> changeFeedContainerProcessorProvider) : IChangeFeedService
{
    private IEnumerable<IContainerChangeFeedProcessor> _processors = new List<IContainerChangeFeedProcessor>();

    public Task StartAsync(CancellationToken _)
    {
        _processors = changeFeedContainerProcessorProvider
            .SelectMany(x => x.GetProcessors())
            .ToList();

        return Task.WhenAll(
            _processors.Select(
                x => x.StartAsync()));
    }

    public async Task StopAsync()
    {
        foreach (IContainerChangeFeedProcessor processor in _processors)
        {
            if (processor is not null)
            {
                await processor.StopAsync();
            }
        }
    }
}
