// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

internal class DefaultChangeFeedContainerProcessorProvider(
    IOptionsMonitor<RepositoryOptions> optionsMonitor,
    ICosmosContainerService containerService,
    ILeaseContainerProvider leaseContainerProvider,
    ILoggerFactory loggerFactory,
    IServiceProvider serviceProvider,
    IChangeFeedOptionsProvider changeFeedOptionsProvider) : IChangeFeedContainerProcessorProvider
{
    public IEnumerable<IContainerChangeFeedProcessor> GetProcessors()
    {
        RepositoryOptions repositoryOptions = optionsMonitor.CurrentValue;

        IEnumerable<IGrouping<string, ContainerOptionsBuilder>> containers = repositoryOptions
            .ContainerBuilder
            .Options
            .Where(x =>
                x.ChangeFeedOptions != null && x.Name is not null)
            .GroupBy(x => x.Name!);

        foreach (IGrouping<string, ContainerOptionsBuilder> container in containers)
        {
            var itemTypes = container.Select(x => x.Type).ToList();

            yield return new DefaultContainerChangeFeedProcessor(
                containerService,
                itemTypes,
                leaseContainerProvider,
                changeFeedOptionsProvider.GetOptionsForItems(itemTypes),
                loggerFactory.CreateLogger<DefaultContainerChangeFeedProcessor>(),
                serviceProvider);
        }
    }
}