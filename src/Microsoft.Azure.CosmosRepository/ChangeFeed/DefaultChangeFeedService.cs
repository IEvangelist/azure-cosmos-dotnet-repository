// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed;

class DefaultChangeFeedService : IChangeFeedService
{
    private readonly IEnumerable<IChangeFeedContainerProcessorProvider> _changeFeedContainerProcessorProvider;
    private IEnumerable<IContainerChangeFeedProcessor> _processors;

    public DefaultChangeFeedService(IEnumerable<IChangeFeedContainerProcessorProvider> changeFeedContainerProcessorProvider)
    {
        _processors = new List<IContainerChangeFeedProcessor>();
        _changeFeedContainerProcessorProvider = changeFeedContainerProcessorProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _processors = _changeFeedContainerProcessorProvider.SelectMany(x => x.GetProcessors());

        cancellationToken.Register(() => StopAsync().Wait(TimeSpan.FromSeconds(5)));

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