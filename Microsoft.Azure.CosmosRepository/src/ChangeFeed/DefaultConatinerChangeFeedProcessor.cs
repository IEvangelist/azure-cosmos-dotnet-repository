// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed
{
    internal class DefaultContainerChangeFeedProcessor : IContainerChangeFeedProcessor
    {
        private readonly ICosmosContainerService _containerService;
        private readonly ILeaseContainerProvider _leaseContainerProvider;
        private readonly ChangeFeedOptions _changeFeedOptions;
        private readonly ILogger<DefaultContainerChangeFeedProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;
        private ChangeFeedProcessor _processor;
        private static readonly ConcurrentDictionary<Type, Type> Handlers = new();

        public DefaultContainerChangeFeedProcessor(
            ICosmosContainerService containerService,
            IReadOnlyList<Type> itemTypes,
            ILeaseContainerProvider leaseContainerProvider,
            ChangeFeedOptions changeFeedOptions,
            ILogger<DefaultContainerChangeFeedProcessor> logger,
            IServiceProvider serviceProvider)
        {
            itemTypes.AreAllItems();
            _containerService = containerService;
            ItemTypes = itemTypes;
            _leaseContainerProvider = leaseContainerProvider;
            _changeFeedOptions = changeFeedOptions;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IReadOnlyList<Type> ItemTypes { get; }

        public async Task StartAsync()
        {
            Container itemContainer = await _containerService.GetContainerAsync(ItemTypes);
            Container leaseContainer = await _leaseContainerProvider.GetLeaseContainerAsync();

            _processor = itemContainer.GetChangeFeedProcessorBuilder<JObject>("cosmos-repository-pattern-processor",
                    (changes, token) => OnChanges(changes, token, itemContainer.Id))
                .WithLeaseContainer(leaseContainer)
                .WithInstanceName(_changeFeedOptions.InstanceName)
                .WithErrorNotification((token, exception) => OnError(exception, itemContainer.Id))
                .Build();

            _logger.LogInformation("Starting change feed processor for container {ContainerName}", itemContainer.Id);

            await _processor.StartAsync();

            _logger.LogInformation("Successfully started change feed processor for container {ContainerName}",
                itemContainer.Id);
        }

        internal async Task OnChanges(IReadOnlyCollection<JObject> changes, CancellationToken cancellationToken,
            string containerName)
        {
            _logger.LogDebug("Detected changes for container {ContainerName} total ({ChangesCount})",
                containerName, changes.Count);

            foreach (JObject change in changes)
            {
                if (!change.TryGetValue("type", out JToken type))
                {
                    //TODO: _logger.LogWarning(...) here? Should this be turn on/off thing? A user might not have created documents with the type field ?!
                    continue;
                }

                Type itemType = ItemTypes.FirstOrDefault(x => x.Name == type.Value<string>());

                if (itemType is null)
                {
                    _logger.LogDebug(
                        "No change feed processor registered for type {ItemType} in container {ContainerName}",
                        type.Value<string>(), containerName);

                    continue;
                }

                await InvokeHandler(itemType, change, cancellationToken);
            }
        }

        private async Task InvokeHandler(Type itemType, JObject instance, CancellationToken cancellationToken)
        {
            object item = instance.ToObject(itemType);

            Type handlerType = null;

            if (Handlers.ContainsKey(itemType) is false)
            {
                handlerType = typeof(IItemChangeFeedProcessor<>).MakeGenericType(itemType);
                Handlers[itemType] = handlerType;
            }

            handlerType ??= Handlers[itemType];

            IEnumerable<object> handlers = _serviceProvider.GetServices(handlerType).ToList();

            _logger.LogDebug("Invoking IItemChangeFeedProcessor's ({ProcessorsCount}) for item type {ItemType}",
                handlers.Count(), itemType);

            foreach (object handler in handlers)
            {
                try
                {
                    object response = handlerType.GetMethod("HandleAsync")?
                        .Invoke(handler, new[] {item, cancellationToken});

                    if (response is ValueTask valueTask)
                    {
                        await valueTask;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "Failed to handle change for item of type {ItemType} when invoking IItemChangeFeedProcessor {ProcessorTypeName}",
                        itemType, handler.GetType().Name);
                }
            }
        }

        private Task OnError(Exception exception, string containerName)
        {
            _logger.LogError(exception, "Failed handling when handling changes detected from container {ContainerName}",
                containerName);
            return Task.CompletedTask;
        }

        public Task StopAsync() =>
            _processor?.StopAsync() ?? Task.CompletedTask;
    }
}