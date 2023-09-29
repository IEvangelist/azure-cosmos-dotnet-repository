// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices.Context;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.ChangeFeed;

internal class DefaultEventSourcingProcessor<TSourcedEvent, TProjectionKey>(
    EventSourcingProcessorOptions<TSourcedEvent, TProjectionKey> options,
    ICosmosContainerService containerService,
    ILeaseContainerProvider leaseContainerProvider,
    ILogger<DefaultEventSourcingProcessor<TSourcedEvent, TProjectionKey>> logger,
    IServiceProvider serviceProvider) : IEventSourcingProcessor
    where TSourcedEvent : EventItem
    where TProjectionKey : IProjectionKey
{
    private ChangeFeedProcessor? _processor;

    public async Task StartAsync()
    {
        Container itemContainer = await containerService.GetContainerAsync<TSourcedEvent>();
        Container leaseContainer = await leaseContainerProvider.GetLeaseContainerAsync();

        ChangeFeedProcessorBuilder builder = itemContainer
            .GetChangeFeedProcessorBuilder<TSourcedEvent>(options.ProcessorName, (changes, token) =>
                OnChangesAsync(changes, itemContainer.Id, token))
            .WithLeaseContainer(leaseContainer)
            .WithInstanceName(options.InstanceName)
            .WithErrorNotification((_, exception) => OnErrorAsync(exception, itemContainer.Id));

        if (options.PollInterval.HasValue)
        {
            builder.WithPollInterval(options.PollInterval.Value);
        }

        _processor = builder.Build();

        logger.LogInformation("Starting change feed processor for container {ContainerName} with key {ProjectionKey} and processor name {ProcessorName}",
            itemContainer.Id,
            typeof(TProjectionKey).Name,
            options.ProcessorName);

        await _processor.StartAsync();

        logger.LogInformation("Successfully started change feed processor for container {ContainerName} with key {ProjectionKey} and processor name {ProcessorName}",
            itemContainer.Id,
            typeof(TProjectionKey).Name,
            options.ProcessorName);
    }

    private async Task OnChangesAsync(
        IReadOnlyCollection<TSourcedEvent> changes,
        string containerName,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Detected changes for container {ContainerName} total ({ChangesCount})",
            containerName, changes.Count);

        foreach (TSourcedEvent change in changes)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IEventItemProjection<TSourcedEvent, TProjectionKey> projection = scope.ServiceProvider
                .GetRequiredService<IEventItemProjection<TSourcedEvent, TProjectionKey>>();

            IContextService contextService = scope.ServiceProvider.GetRequiredService<IContextService>();
            contextService.CorrelationId = change.CorrelationId;

            try
            {
                await projection.ProjectAsync(change, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    "Failed handling projection for container {ContainerName} source event ID {SourcedEventId}",
                    containerName, change.Id);
            }
        }
    }

    private Task OnErrorAsync(Exception exception, string containerName)
    {
        logger.LogError(exception, "Failed handling when handling changes detected from container {ContainerName}",
            containerName);
        return Task.CompletedTask;
    }

    public Task StopAsync() =>
        _processor?.StopAsync() ?? Task.CompletedTask;

    public IReadOnlyList<Type> ItemTypes => new[] { typeof(TSourcedEvent) };
}