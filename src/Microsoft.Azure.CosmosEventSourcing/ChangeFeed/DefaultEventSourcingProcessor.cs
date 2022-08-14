// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.ChangeFeed;

internal class DefaultEventSourcingProcessor<TSourcedEvent, TProjectionKey> : IEventSourcingProcessor
    where TSourcedEvent : EventItem
    where TProjectionKey : IProjectionKey
{
    private readonly EventSourcingProcessorOptions<TSourcedEvent, TProjectionKey> _options;
    private readonly ICosmosContainerService _containerService;
    private readonly ILeaseContainerProvider _leaseContainerProvider;
    private readonly ILogger<DefaultEventSourcingProcessor<TSourcedEvent, TProjectionKey>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private ChangeFeedProcessor? _processor;

    public DefaultEventSourcingProcessor(
        EventSourcingProcessorOptions<TSourcedEvent, TProjectionKey> options,
        ICosmosContainerService containerService,
        ILeaseContainerProvider leaseContainerProvider,
        ILogger<DefaultEventSourcingProcessor<TSourcedEvent, TProjectionKey>> logger,
        IServiceProvider serviceProvider)
    {
        _options = options;
        _containerService = containerService;
        _leaseContainerProvider = leaseContainerProvider;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync()
    {
        Container itemContainer = await _containerService.GetContainerAsync<TSourcedEvent>();
        Container leaseContainer = await _leaseContainerProvider.GetLeaseContainerAsync();

        ChangeFeedProcessorBuilder builder = itemContainer
            .GetChangeFeedProcessorBuilder<TSourcedEvent>(_options.ProcessorName, (changes, token) =>
                OnChangesAsync(changes, token, itemContainer.Id))
            .WithLeaseContainer(leaseContainer)
            .WithInstanceName(_options.InstanceName)
            .WithErrorNotification((_, exception) => OnErrorAsync(exception, itemContainer.Id));

        if (_options.PollInterval.HasValue)
        {
            builder.WithPollInterval(_options.PollInterval.Value);
        }

        _processor = builder.Build();

        _logger.LogInformation("Starting change feed processor for container {ContainerName} with key {ProjectionKey} and processor name {ProcessorName}",
            itemContainer.Id,
            typeof(TProjectionKey).Name,
            _options.ProcessorName);

        await _processor.StartAsync();

        _logger.LogInformation("Successfully started change feed processor for container {ContainerName} with key {ProjectionKey} and processor name {ProcessorName}",
            itemContainer.Id,
            typeof(TProjectionKey).Name,
            _options.ProcessorName);
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
            using IServiceScope scope = _serviceProvider.CreateScope();
            IEventItemProjection<TSourcedEvent, TProjectionKey> projection = scope.ServiceProvider
                .GetRequiredService<IEventItemProjection<TSourcedEvent, TProjectionKey>>();

            try
            {
                await projection.ProjectAsync(change, cancellationToken);
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