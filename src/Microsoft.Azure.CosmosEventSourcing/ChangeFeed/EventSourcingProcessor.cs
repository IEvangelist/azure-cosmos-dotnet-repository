// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.ChangeFeed;

public class EventSourcingProcessor<TSourcedEvent> : IEventSourcingProcessor
    where TSourcedEvent : SourcedEvent
{
    private readonly EventSourcingProcessorOptions<TSourcedEvent> _options;
    private readonly ICosmosContainerService _containerService;
    private readonly ILeaseContainerProvider _leaseContainerProvider;
    private readonly ILogger<EventSourcingProcessor<TSourcedEvent>> _logger;
    private readonly ISourceProjectionBuilder<TSourcedEvent> _projectionBuilder;
    private ChangeFeedProcessor? _processor;

    public EventSourcingProcessor(
        EventSourcingProcessorOptions<TSourcedEvent> options,
        ICosmosContainerService containerService,
        ILeaseContainerProvider leaseContainerProvider,
        ILogger<EventSourcingProcessor<TSourcedEvent>> logger,
        ISourceProjectionBuilder<TSourcedEvent> projectionBuilder)
    {
        _options = options;
        _containerService = containerService;
        _leaseContainerProvider = leaseContainerProvider;
        _logger = logger;
        _projectionBuilder = projectionBuilder;
    }

    public async Task StartAsync()
    {
        Container itemContainer = await _containerService.GetContainerAsync<TSourcedEvent>();
        Container leaseContainer = await _leaseContainerProvider.GetLeaseContainerAsync();

        ChangeFeedProcessorBuilder builder = itemContainer
            .GetChangeFeedProcessorBuilder<TSourcedEvent>(_options.ProcessorName,
                (changes, token) => OnChangesAsync(changes, token, itemContainer.Id))
            .WithLeaseContainer(leaseContainer)
            .WithInstanceName(_options.InstanceName)
            .WithErrorNotification((token, exception) => OnErrorAsync(exception, itemContainer.Id));

        if (_options.PollInterval.HasValue)
        {
            builder.WithPollInterval(_options.PollInterval.Value);
        }

        _processor = builder.Build();

        _logger.LogInformation("Starting change feed processor for container {ContainerName}", itemContainer.Id);

        await _processor.StartAsync();

        _logger.LogInformation("Successfully started change feed processor for container {ContainerName}",
            itemContainer.Id);
    }

    private async Task OnChangesAsync(
        IReadOnlyCollection<TSourcedEvent> changes,
        CancellationToken cancellationToken,
        string containerName)
    {
        _logger.LogDebug("Detected changes for container {ContainerName} total ({ChangesCount})",
            containerName, changes.Count);

        foreach (TSourcedEvent change in changes)
        {
            try
            {
                await _projectionBuilder.ProjectAsync(change, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Failed handling projection for container {ContainerName} source event ID {SourcedEventId}",
                    containerName, change.Id);
            }
        }
    }

    private Task OnErrorAsync(Exception exception, string containerName)
    {
        _logger.LogError(exception, "Failed handling when handling changes detected from container {ContainerName}",
            containerName);
        return Task.CompletedTask;
    }

    public Task StopAsync() =>
        _processor?.StopAsync() ?? Task.CompletedTask;

    public IReadOnlyList<Type> ItemTypes => new[] {typeof(TSourcedEvent)};
}