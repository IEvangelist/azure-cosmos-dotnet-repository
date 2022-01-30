// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosEventSourcing.ChangeFeed;

public class EventSourcingProcessor<TSourcedEvent> : IContainerChangeFeedProcessor
    where TSourcedEvent : SourcedEvent
{
    private readonly ICosmosContainerService _containerService;
        private readonly ILeaseContainerProvider _leaseContainerProvider;
        private readonly ILogger<EventSourcingProcessor<TSourcedEvent>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private ChangeFeedProcessor _processor;
        private static readonly ConcurrentDictionary<Type, Type> Handlers = new();

        public EventSourcingProcessor(
            ICosmosContainerService containerService,
            ILeaseContainerProvider leaseContainerProvider,
            ILogger<EventSourcingProcessor<TSourcedEvent>> logger,
            IServiceProvider serviceProvider)
        {
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
                .GetChangeFeedProcessorBuilder<TSourcedEvent>("a",
                    (changes, token) => OnChangesAsync(changes, token, itemContainer.Id))
                .WithLeaseContainer(leaseContainer)
                .WithInstanceName("b")
                .WithErrorNotification((token, exception) => OnErrorAsync(exception, itemContainer.Id));

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

            await Task.WhenAll(changes.Select(x => InvokeHandlerAsync(x.Event, cancellationToken)));
        }

        private async Task InvokeHandlerAsync(IPersistedEvent item,
            CancellationToken cancellationToken)
        {
            Type itemType = item.GetType();

            Type? handlerType = null;

            if (Handlers.ContainsKey(itemType) is false)
            {
                handlerType = typeof(IProjectionBuilder<>).MakeGenericType(itemType);
                Handlers[itemType] = handlerType;
            }

            handlerType ??= Handlers[itemType];

            IEnumerable<object> handlers = _serviceProvider.GetServices(handlerType).ToList();

            _logger.LogDebug("Invoking IProjectionBuilder's ({ProcessorsCount}) for item type {ItemType}",
                handlers.Count(), itemType);

            await Task.WhenAll(handlers.Select(handler =>
            {
                try
                {
                    object? response = handlerType.GetMethod("ProjectAsync")?
                        .Invoke(handler, new object[] {item, cancellationToken});

                    if (response is ValueTask valueTask)
                    {
                        return valueTask.AsTask();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "Failed to handle change for item of type {ItemType} when invoking IProjectionBuilder {ProcessorTypeName}",
                        itemType, handler.GetType().Name);
                }

                return Task.CompletedTask;
            }));
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