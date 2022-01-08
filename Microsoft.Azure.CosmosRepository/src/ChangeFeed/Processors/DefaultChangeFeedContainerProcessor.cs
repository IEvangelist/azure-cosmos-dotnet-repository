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

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Processors
{
    internal class DefaultChangeFeedContainerProcessor : IChangeFeedContainerProcessor
    {
        private readonly ICosmosContainerService _containerService;
        private readonly IReadOnlyList<Type> _itemTypes;
        private readonly ILeaseContainerProvider _leaseContainerProvider;
        private readonly ILogger<DefaultChangeFeedContainerProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;
        private ChangeFeedProcessor _processor;
        private static readonly ConcurrentDictionary<Type, Type> Handlers = new();

        public DefaultChangeFeedContainerProcessor(ICosmosContainerService containerService,
            IReadOnlyList<Type> itemTypes, ILeaseContainerProvider leaseContainerProvider,
            ILogger<DefaultChangeFeedContainerProcessor> logger,
            IServiceProvider serviceProvider)
        {
            itemTypes.EnsureAllAreTypeOfIItem();
            _containerService = containerService;
            _itemTypes = itemTypes;
            _leaseContainerProvider = leaseContainerProvider;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync()
        {
            Container itemContainer = await _containerService.GetContainerAsync(_itemTypes);
            Container leaseContainer = await _leaseContainerProvider.GetLeaseContainerAsync();

            _processor = itemContainer.GetChangeFeedProcessorBuilder<JObject>("cosmos-repository-pattern-processor",
                    (changes, token) => OnChanges(changes, token, itemContainer.Id))
                .WithLeaseContainer(leaseContainer)
                .WithInstanceName("?")
                .WithErrorNotification((token, exception) => OnError(exception, itemContainer.Id))
                .Build();

            _logger.LogInformation("Starting change feed processor for container {ContainerName}", itemContainer.Id);

            await _processor.StartAsync();

            _logger.LogInformation("Successfully started change feed processor for container {ContainerName}",
                itemContainer.Id);
        }

        internal async Task OnChanges(IReadOnlyCollection<JObject> changes, CancellationToken cancellationtoken,
            string containerName)
        {
            _logger.LogDebug("Detected ({ChangesCount}) changes for container {ContainerName}", changes.Count,
                containerName);

            foreach (JObject change in changes)
            {
                if (!change.TryGetValue("type", out JToken type))
                {
                    //TODO: _logger.LogWarning(...) here? Should this be turn on/off thing?
                    continue;
                }

                Type itemType = _itemTypes.FirstOrDefault(x => x.Name == type.Value<string>());

                if (itemType is null)
                {
                    //TODO: _logger.LogWarning(...) here? Should this be turn on/off thing? Or Should we do nothing?
                    continue;
                }

                await InvokeHandler(itemType, change, cancellationtoken);
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

            object response = handlerType.GetMethod("HandleAsync")?
                .Invoke(_serviceProvider.GetRequiredService(handlerType), new[] {item, cancellationToken});

            if (response is ValueTask valueTask)
            {
                await valueTask;
            }
        }

        private Task OnError(Exception exception, string containerName)
        {
            _logger.LogError(exception, "Failed handling when handling changes detected from container {ContainerName}",
                containerName);
            return Task.CompletedTask;
        }

        public Task StopAsync() =>
            _processor is not null ? _processor.StopAsync() : Task.CompletedTask;
    }
}